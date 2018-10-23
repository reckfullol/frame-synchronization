using CommonLib;
using CommonLib.Utility;
using UnityEngine;

namespace MainClient.UI.BattleUI {
    public class BattleUIManager : Singleton<BattleUIManager> {
        private GameObject _mainGo = null;

        public override bool Init() {
            Object obj = Resources.Load("Prefabs/ui/BattleUI");
            _mainGo = Object.Instantiate(obj) as GameObject;
            if (_mainGo != null) {
                _mainGo.transform.SetParent(GameClient.Instance.mainCanvas, false);
                DirectionUI.MainGo = _mainGo.transform.Find("TouchDirctionArea").gameObject;
                AttackUI.MainGo = _mainGo.transform.Find("TouchFunction").gameObject;
            }

            DirectionUI.Enable = false;
            AttackUI.Enable = false;
            return true;
        }

        public DirectionUIManager DirectionUI { get; } = new DirectionUIManager();

        public AttackButtonUIManager AttackUI { get; } = new AttackButtonUIManager();
    }
}
