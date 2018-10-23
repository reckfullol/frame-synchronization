using UnityEngine;
using System.Collections;
using CommonLib.Utility;
using MainClient.Camera;
using MainClient.Character;
using MainClient.GameInput;
using MainClient.GameSync;
using MainClient.Network;
using MainClient.Network.Protocol;
using MainClient.Network.Protocol.Request;
using MainClient.Ticker;
using MainClient.UI.BattleUI;

namespace MainClient
{
    internal class GameClient : Singleton<GameClient>
    {
        private GameObject _mainCamera;
        private Vector3 _mainCameraOffset;
        public RectTransform mainCanvas;
        private Vector2 _halfMainCanvasSize;

        private IEnumerator _launcher = null;

        private bool IsStart { get; set; } = false;

        public InputHandle InputHandle { get; } = new InputHandle();

        public CharacterBase MainPlayer { get; private set; } = null;

        private static IEnumerator LaunchGame() {
            yield return null;
            CharacterConfigManager.Instance.Init();
        }

        #region ITick interface
        public void Update(float delta) {
            if (!IsStart) {
                if (_launcher == null) {
                    _launcher = LaunchGame();
                } else {
                    if (_launcher.MoveNext()) {
                        return;
                    } 
                    _launcher = null;
                    IsStart = true;
                    RealStart();
                }
                return;
            }
            GameSyncManager.Instance.Update(delta);
            InputHandle.Update(delta);
            ClientNetworkManager.Instance.Update(delta);
            TickerManager.Instance.Update(delta);
        }

        public void LateUpdate() {
            if (!IsStart) {
                return;
            }
            TickerManager.Instance.LateUpdate();
            CameraManager.Instance.Update(Time.deltaTime);
        }
        #endregion

        #region IFixedTick interface
        public void FixedUpdate() {
            if (!IsStart) {
                return;
            }
            // 帧同步模式直接跳过
            if (!GameSyncManager.Instance.IsStepLockMode) {
               // FixedUpdate涉及到同步，所有需要同步的只能加到TickerManager里面
               // 其他地方不能调用
                TickerManager.Instance.FixedUpdate();
            }
        }
        #endregion

        public override bool Init() {
            IsStart = false;
            Time.fixedDeltaTime = CommonFunction.GAME_TIME_PRE_FRAME;
            ProtocolRegister.RegistProtocol();
            GameObject go = GameObject.FindGameObjectWithTag("MainCanvas");
            mainCanvas = go.GetComponent<RectTransform>();
            _halfMainCanvasSize = mainCanvas.rect.size / 2f;
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _mainCameraOffset = _mainCamera.transform.position;
            CameraManager.Instance.SetCamera(_mainCamera.transform, _mainCameraOffset);
            return true;
        }

        private void RealStart() {
            BattleUIManager.Instance.Init();

            MainPlayer = CharacterManager.Instance.GetNewCharacter();
            InputHandle.AddInputListener(MainPlayer);
            MainPlayer.ChangeCharacter(1);

//            ClientNetworkManager.Instance.Connect("127.0.0.1", 5678);

            Test();
        }

        public static void OnNetworkConnect() {
            Debug.Log("Connect success");
            ProtolRequestStart pb = new ProtolRequestStart();
            ClientNetworkManager.Instance.Send(pb);
        }

        public static void OnLoginSuccess(ulong uin) {
            GameSyncManager.Instance.Uin = uin;
            Debug.Log("login success uin = " + uin.ToString());
        }

        private void Test() {
            GameSyncManager.Instance.BeforeStartGame(SyncMode.NORMAL);
            MainPlayer.ThisData.uin = 1;
            MainPlayer.Position = new Vector3(5f, 0f, 5f);
            CharacterBase testEnemy = CharacterManager.Instance.GetNewCharacter();
            testEnemy.ChangeCharacter(1);
            testEnemy.Position = Vector3.zero;
            testEnemy.ThisData.uin = 2;
            InputHandle.AddInputListener(MainPlayer);
            TickerManager.Instance.AddTick(MainPlayer);
            TickerManager.Instance.AddFixedTick(MainPlayer);
            TickerManager.Instance.AddTick(testEnemy);
            TickerManager.Instance.AddFixedTick(testEnemy);

            GameSyncManager.Instance.AfterStartGame();
        }

        public static void OnApplicationQuit() {
            ClientNetworkManager.Instance.Close();
        }

        public Vector2 GetPointInCanvas(Vector2 pos) {
            Vector2 touchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, pos, null, out touchPos);
            touchPos += _halfMainCanvasSize;
            return touchPos;
        }
    }
}
