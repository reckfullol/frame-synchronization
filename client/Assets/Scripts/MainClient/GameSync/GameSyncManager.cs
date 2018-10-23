using System;
using System.Collections.Generic;
using CommonLib;
using CommonLib.Utility;
using MainClient.Camera;
using MainClient.Character;
using MainClient.GameInput;
using MainClient.Network;
using MainClient.Network.Protocol.Request;
using MainClient.Ticker;
using MainClient.UI.BattleUI;
using UnityEngine;

namespace MainClient.GameSync {
    internal enum SyncMode {
        NORMAL,
        STEP_LOCK,
    }

    internal class GameSyncManager : Singleton<GameSyncManager> {
        private uint _nextFrameSetp = 0;

        private readonly ProtocolFrameNotify _frameUpdateReq;

        private readonly Dictionary<ulong, IInputListener> _inputListeners;

        private bool _needUpdate = false;

        private bool _isGameing = false;

        #region 基础属性
        public bool IsStepLockMode => CurrentMode == SyncMode.STEP_LOCK;

        private SyncMode CurrentMode { get; set; } = SyncMode.NORMAL;

        public ulong Uin { private get; set; } = 0;

        #endregion

        public void Update(float detla) {
            if (!_isGameing || !IsStepLockMode || !_needUpdate) {
                return;
            }
            ClientNetworkManager.Instance.Send(_frameUpdateReq);
            _frameUpdateReq.data.Keys.Clear();
            _needUpdate = false;
        }

        public GameSyncManager() {
            _frameUpdateReq = new ProtocolFrameNotify();
            _inputListeners = new Dictionary<ulong, IInputListener>();
        }

        public void BeforeStartGame(SyncMode mode) {
            CurrentMode = mode;

            // 初始化数据
            TickerManager.Instance.ClearAllTicks();
            CharacterManager.Instance.Clear();
            // 第一帧是1
            _nextFrameSetp = 1;
            TickerManager.Instance.FiexTickCount = 0;
            _frameUpdateReq.data.Uin = Uin;
            _frameUpdateReq.data.Keys.Clear();
            _inputListeners.Clear();
            _needUpdate = false;
            BattleUIManager.Instance.DirectionUI.Enable = true;
            BattleUIManager.Instance.AttackUI.Enable = true;
            CameraManager.Instance.SetTarget(GameClient.Instance.MainPlayer.ThisView.MainTransform);
        }

        public void AfterStartGame() {
            _isGameing = true;
        }

        public void StartBattle(SCStartGame res) {
            BeforeStartGame(SyncMode.STEP_LOCK);
            CharacterBase mainPlayer = GameClient.Instance.MainPlayer;
            foreach (var p in res.Uins) {
                CharacterBase currentPlayer = null;
                currentPlayer = p == Uin ? mainPlayer : CharacterManager.Instance.GetNewCharacter();
                currentPlayer.ThisData.uin = p;

                currentPlayer.ChangeCharacter(1);
                currentPlayer.Init();
                _inputListeners.Add(currentPlayer.ThisData.uin, currentPlayer);
                currentPlayer.Position = Vector3.zero;
                TickerManager.Instance.AddTick(currentPlayer);
                TickerManager.Instance.AddFixedTick(currentPlayer);
            }
            AfterStartGame();
        }

        #region 帧同步信息
        public void AddKeyInfo(ushort keyCode, ushort value, byte type) {
            if (!_isGameing) {
                return;
            }
            keyCode &= 0xF;
            value &= 0x3FF;
            type &= 0x3;
            keyCode <<= 10;
            keyCode |= value;
            keyCode <<= 2;
            keyCode |= type;
            _frameUpdateReq.data.Keys.Add(keyCode);
            _needUpdate = true;
        }

        public void OnFreamAsyn(SCFrameNotify frameAsyn) {
            if (!_isGameing) {
                return;
            }
            // 无操作的帧， 直接跑过去
            if (_nextFrameSetp < frameAsyn.CurrentFrame) {
                TickerManager.Instance.DriveToTickCount(frameAsyn.CurrentFrame - 1);
                _nextFrameSetp = frameAsyn.CurrentFrame;
            }
            // 这个是我们需要的
            if (_nextFrameSetp != frameAsyn.CurrentFrame) {
                return;
            }
            // 按按键
            foreach (CSFrameNotify frameControlList in frameAsyn.Keys) {
                if (_inputListeners.ContainsKey(frameControlList.Uin)) {
                    IInputListener listener = _inputListeners[frameControlList.Uin];
                    foreach (var con in frameControlList.Keys) {
                        #region 解析按键
                        ushort key = (ushort)con;
                        ushort type = (byte)(key & 0x3);
                        key >>= 2;
                        ushort value = (ushort)(key & 0x3FF);
                        key >>= 10;
                        key &= 0xF;
                        #endregion
                        if (key == InputDefine.DIRCTION_KEY) {
                            float ang = value;
                            if (CommonFunction.GreatOrEqualZero(ang - 180f)) {
                                ang -= 360f;
                            }
                            listener.OnMove(ang, (MoveType)type);
                        } else {
                            if (type > 0) {
                                listener.OnFunctionKeyDown(key);
                            } else {
                                listener.OnFunctionKeyUp(key);
                            }
                        }
                    }
                }
            }
            _nextFrameSetp = frameAsyn.NextFrame;
            TickerManager.Instance.DriveToTickCount(_nextFrameSetp - 1);
        }

        #endregion
    }
}
