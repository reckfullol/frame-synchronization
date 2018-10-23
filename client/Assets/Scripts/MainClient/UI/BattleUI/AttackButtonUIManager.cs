using MainClient.GameInput;
using UnityEngine;
using UnityEngine.UI;

namespace MainClient.UI.BattleUI {
    public class AttackButtonUIManager {
        private GameObject _mainGo = null;

        public GameObject MainGo {
            set {
                if (_mainGo != null || value == null) {
                    return;
                }
                _mainGo = value;

                Button normalAtk = _mainGo.transform.Find("NormalAtk").GetComponent<Button>();
                normalAtk.onClick.AddListener(OnNormalAttackUIPointClick);

                Enable = false;
            }
        }

        public bool Enable {
            set {
                if (_mainGo != null) {
                    _mainGo.SetActive(value);
                }
            }
        }

        private static void OnNormalAttackUIPointClick() {
            GameClient.Instance.InputHandle.FunctionKeyDown(InputDefine.FUNCTION_KEY1);
        }
    }
}