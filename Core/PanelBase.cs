using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace UI_Frame.Core {
    public class PanelBase : MonoBehaviour {

        //子类中需要按照以下格式来声明面板控件
        //public GameObject UI_xxx; //xxx必须跟面板中的空间名字一样
        //因为是GameObject，所以访问组件特别麻烦...

        //在子类中赋值
        public string skinPath;
        public PanelLayer layer;

        //脚本对应的面板资源
        public GameObject skin;

        //Init是用于自定义数据的初始化
        public object[] args;
        //params？？？不定长参数
        public virtual void Init(params object[] args) {
            this.args = args;
        }

        #region 声明周期
        public virtual void OnShowing() {
            RectTransform skinTrans = skin.transform as RectTransform;
            RectTransform[] children = skinTrans.GetComponentsInChildren<RectTransform>();

            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields) {
                if (field.Name.Substring(0, 3) == "UI_") {
                    string UI_Name = field.Name.Substring(3);
                    foreach (RectTransform child in children) {
                        if (child.name == UI_Name) {
                            field.SetValue(this, child.gameObject);
                            break;
                        }
                    } 
                }
            }
        }
        public virtual void OnShowed() { }
        public virtual void Update() { }
        public virtual void OnClosing() { }
        public virtual void OnClosed() { }
        #endregion

        #region helper funtion
        protected virtual void Close() {
            string name = this.GetType().Name;
            PanelMgr.instance.ClosePanel(name);
        }
        #endregion
    }
}