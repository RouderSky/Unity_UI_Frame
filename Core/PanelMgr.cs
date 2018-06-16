using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI_Frame.Core {

    //层级示例:Panel_test_ok
    //越下层，等级越高
    public enum PanelLayer {
        Panel,
        Tips,
    }

    //为什么这个单例这么奇怪？？？
    public class PanelMgr : MonoBehaviour {

        public static PanelMgr instance;
        private GameObject canvas;
        private Dictionary<PanelLayer, Transform> layerDict;        //存放层级对应的父物体
        //访问特定面板的特定属性特别麻烦...
        public Dictionary<string, PanelBase> panelDict;             //存放已打开的面板实例

        //为何重写了Awake，这个脚本就不能enable=false？
        //为什么改成Start后就会报错，是脚本执行顺序的问题吗？
        public void Awake() {
            instance = this;

            //寻找画布
            canvas = GameObject.Find("Canvas");
            if (canvas == null)
                Debug.LogError("panelMgr,InitLayer fail, canvas is null");

            //建立层级
            //依照枚类型在游戏中查找相应层级，并记录到字典
            layerDict = new Dictionary<PanelLayer, Transform>();
            foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer))) {
                string name = pl.ToString();
                string[] strs = name.Split('_');
                Transform curTrans = canvas.transform;
                foreach (string str in strs) {
                    Transform nextTrans = curTrans.Find(str);
                    if (nextTrans == null) {
                        GameObject obj = new GameObject(str);
                        obj.transform.SetParent(curTrans);
                        obj.transform.localPosition = new Vector3(0, 0, 0);
                        obj.AddComponent<RectTransform>();
                        obj.transform.localScale = new Vector3(1, 1, 1);
                        curTrans = obj.transform;
                    }
                    else {
                        curTrans = nextTrans;
                    }
                }

                layerDict.Add(pl, curTrans);
            }

            panelDict = new Dictionary<string, PanelBase>();
        }

        //一个脚本只能存在一个实例
        //一个脚本可以使用不同面板资源，skinPath可以临时自定义面板资源
        public void OpenPanel<T>(string skinPath = "", params object[] args) where T : PanelBase {
            //检查面板实例是否已经创建，这样的话，一个面板在游戏中只能打开一个？
            //string name = typeof(T).ToString();
            string name = typeof(T).Name;
            if (panelDict.ContainsKey(name)) {
                PanelBase oldPanel = panelDict[name];
                //oldPanel.enabled = true;      //和GameObject的SetActive有什么区别？？？作用对象不同
                //oldPanel.skin.SetActive(true);
                int count = oldPanel.skin.transform.parent.childCount;
                oldPanel.skin.transform.SetSiblingIndex(count - 1);
                return;
            }

            //创建面板实例
            PanelBase newPanel = canvas.AddComponent<T>();
            newPanel.Init(args);
            panelDict.Add(name, newPanel);

            //为面板实例创建面板资源
            skinPath = (skinPath != "" ? skinPath : newPanel.skinPath);
            GameObject skin = Resources.Load<GameObject>(skinPath);
            if (skin == null)
                Debug.LogError("panelMgr.OpenPanel fail, skin is null,skinPath = " + skinPath);
            newPanel.skin = (GameObject)Instantiate(skin);

            //设置面板资源层级
            Transform skinTrans = newPanel.skin.transform;
            PanelLayer layer = newPanel.layer;
            Transform parent = layerDict[layer];
            skinTrans.SetParent(parent, false);

            //自定义
            newPanel.OnShowing();
            newPanel.OnShowed();
        }

        public void ClosePanel(string name) {
            PanelBase panel = (PanelBase)panelDict[name];
            if (panel == null)
                return;

            panel.OnClosing();
            panelDict.Remove(name);
            panel.OnClosed();
            GameObject.Destroy(panel.skin);
            //panel.skin.SetActive(false);
            Component.Destroy(panel);
            //panel.enabled = false;
        }

        //这个函数有问题，遍历的过程在修改字典，会报out of sync
        //待Debug...
        public void Clear() {
            lock (panelDict) {
                foreach (string name in panelDict.Keys)
                    ClosePanel(name);
            }
        }
    }
}
