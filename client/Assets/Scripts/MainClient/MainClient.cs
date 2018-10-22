using CommonLib;
using System;
using UnityEngine;
using System.Collections;

namespace MainClient
{
    class GameClient : Singleton<GameClient>
    {
        public GameObject mainCamera;
        private Vector3 _mainCameraOffset;
        public RectTransform mainCanvas;
        public Vector2 halfMainCanvasSize;

        private CharacterBase _mainPlayer = null;
        private InputHandle _inputHandle = new InputHandle();

        private bool _isStart = false;

        private IEnumerator _launcher = null;

        public bool IsStart
        {
            get
            {
                return _isStart;
            }
        }
        public InputHandle InputHandle
        {
            get
            {
                return _inputHandle;
            }
        }
        public CharacterBase MainPlayer
        {
            get
            {
                return _mainPlayer;
            }
        }

        private IEnumerator LaunchGame()
        {
            yield return null;
            CharacterConfigManager.Instance.Init();
        }

        #region ITick interface
        public void Update(float delta)
        {
            if (!_isStart)
            {
                if (_launcher == null)
                {
                    _launcher = LaunchGame();
                }
                else
                {
                    if (!_launcher.MoveNext())
                    {
                        _launcher = null;
                        _isStart = true;
                        RealStart();
                    }
                }
                return;
            }
            GameSyncManager.Instance.Update(delta);
            _inputHandle.Update(delta);
            ClientNetworkManager.Instance.Update(delta);
            TickerManager.Instance.Update(delta);
        }

        public void LateUpdate()
        {
            if (!_isStart)
            {
                return;
            }
            TickerManager.Instance.LateUpdate();
            CameraManager.Instance.Update(Time.deltaTime);
        }
        #endregion

        #region IFixedTick interface
        public void FixedUpdate()
        {
            if (!_isStart)
            {
                return;
            }
            // 帧同步模式直接跳过
            if (!GameSyncManager.Instance.IsStepLockMode)
            {
               // FixedUpdate涉及到同步，所有需要同步的只能加到TickerManager里面
               // 其他地方不能调用
                TickerManager.Instance.FixedUpdate();
            }
        }
        #endregion

        public override bool Init()
        {
            _isStart = false;
            Time.fixedDeltaTime = CommonFunction.GAME_TIME_PRE_FRAME;
            PtcRegister.RegistProtocol();
            GameObject go = GameObject.FindGameObjectWithTag("MainCanvas");
            mainCanvas = go.GetComponent<RectTransform>();
            halfMainCanvasSize = mainCanvas.rect.size / 2f;
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _mainCameraOffset = mainCamera.transform.position;
            CameraManager.Instance.SetCamera(mainCamera.transform, _mainCameraOffset);
            return true;
        }

        private void RealStart()
        {
            BattleUIManager.Instance.Init();

            _mainPlayer = CharacterManager.Instance.GetNewCharacter();
            _inputHandle.AddInputListener(_mainPlayer);
            _mainPlayer.ChangeCharacter(1);

//            ClientNetworkManager.Instance.Connect("127.0.0.1", 5678);

            Test();
        }

        public void OnNetworkConnect()
        {
            Debug.Log("Connect success");
            var pb = new ProtolRequestStart();
            ClientNetworkManager.Instance.Send(pb);
        }

        public void OnLoginSuccess(UInt64 uin)
        {
            GameSyncManager.Instance.Uin = uin;
            Debug.Log("login success uin = " + uin.ToString());
        }

        public void Test()
        {
            GameSyncManager.Instance.BeforeStartGame(SyncMode.NORMAL);
            _mainPlayer.ThisData.uin = 1;
            _mainPlayer.Position = new Vector3(5f, 0f, 5f);
            CharacterBase testEnemy = CharacterManager.Instance.GetNewCharacter();
            testEnemy.ChangeCharacter(1);
            testEnemy.Position = Vector3.zero;
            testEnemy.ThisData.uin = 2;
            _inputHandle.AddInputListener(_mainPlayer);
            TickerManager.Instance.AddTick(_mainPlayer);
            TickerManager.Instance.AddFixedTick(_mainPlayer);
            TickerManager.Instance.AddTick(testEnemy);
            TickerManager.Instance.AddFixedTick(testEnemy);

            GameSyncManager.Instance.AfterStartGame();
        }

        public void OnApplicationQuit()
        {
            ClientNetworkManager.Instance.Close();
        }

        public Vector2 GetPointInCanvas(Vector2 pos)
        {
            Vector2 touchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, pos, null, out touchPos);
            touchPos += halfMainCanvasSize;
            return touchPos;
        }
    }
}
