using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UI_Frame.Core;

namespace UI_Frame.Logic {
    public class TipPanel : PanelBase {
        public GameObject UI_TipsText;
        public GameObject UI_CheckBtn;

        string content = "";

        #region 生命周期
        public override void Init(params object[] args) {
            base.Init(args);
            skinPath = "TipPanel";
            layer = PanelLayer.Tips;
            if (args.Length == 1)
                content = (string)args[0];
        }

        public override void OnShowing() {
            base.OnShowing();

            UI_TipsText.GetComponent<Text>().text = content;
            UI_CheckBtn.GetComponent<Button>().onClick.AddListener(OnCheckBtnClick);
        }
        #endregion
        void OnCheckBtnClick() {
            Close();
        }
    }
}