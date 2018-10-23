using System;
using CommonLib;
using CommonLib.Interface;
using CommonLib.Utility;
using MainClient.GameInput;
using MainClient.Ticker;
using UnityEngine;

namespace MainClient.Character {
    public class CharacterBase : IInputListener, IFixedTick, ITick, IObjectPool {
        private CharacterMoveUnit _move = null;

        #region 构造以及初始化
        /// <summary>
        /// 不要主动调用，通过CharacterManager.Instance.GetNewCharacter();创建
        /// </summary>
        public CharacterBase() {
            _move = new CharacterMoveUnit(this);
            ThisView = new CharacterView();
            ThisActionUnit = new CharacterActionUnit(this);
            ThisData = new CharacterDataUnit();
        }
        public void Init() {
            _move.Init();
            ThisView.Init();
            ThisData.Init();
            ThisActionUnit.Init();
            OnMove(0f, MoveType.STOP);
        }
        #endregion
        #region 切换资源
        public void ChangeCharacter(uint roleID) {
            if (ThisConfig == null || ThisConfig.characterID != roleID) {
                CharacterConfig config = CharacterConfigManager.Instance.GetCharacterConfig(roleID);
                if (config != null) {
                    ThisView.ChangeView(config.resName, config);
                    ThisConfig = config;
                } else {
                    return;
                }
            }
            ThisActionUnit.ChangeAction(CharacterAction.IDLE, true);
            Init();
        }
        #endregion

        #region 基础数据相关
        public CharacterDataUnit ThisData { get; } = null;

        private CharacterActionUnit ThisActionUnit { get; } = null;

        public CharacterView ThisView { get; } = null;

        public CharacterConfig ThisConfig { get; private set; } = null;

        #endregion

        #region 速度相关
        private void UpdateForwardSpeed() {
            _move.forwardSpeed = 0f;
            float moveSpeed = 0f;
            if (ThisData.lastMoveType == MoveType.PAUSE) {

            } else if (ThisActionUnit.CurrentMoveAction == CharacterAction.RUN) {
                moveSpeed = ThisConfig.runSpeed;
            }
            if (ThisActionUnit.CurrentActionConfig.canRotate) {
                _move.forwardSpeed = moveSpeed;
            }
        }
        #endregion

        #region 动作相关
        public void OnActionChanged() {
            UpdateForwardSpeed();
            _move.UpdateViewPosition();

            if (ThisActionUnit.CurrentActionConfig.canRotate || ThisActionUnit.CurrentAction == CharacterAction.IDLE) {
                _move.Update();
            }
        }
        #endregion

        #region IInputListener intercace
        #region 方向键按键相关
        public void OnMove(float ange, MoveType moveType) {
            switch (moveType) {
                case MoveType.NONE:
                    return;
                case MoveType.STOP:
                    ThisActionUnit.ChangeMoveAction(CharacterAction.IDLE);
                    break;
                default:
                    CharacterAction targetAction = CharacterAction.RUN;
                    float dest = ange;
                    if (CommonFunction.LessZero(dest)) {
                        dest += 360f;
                    }
                    if (ThisActionUnit.CurrentMoveAction == targetAction && CommonFunction.IsZero(_move.Rotation.y - dest) && 
                        ThisData.lastMoveType == MoveType.MOVE_ANGE && moveType == MoveType.MOVE_ANGE) {
                        return;
                    }
                    ThisActionUnit.ChangeMoveAction(targetAction);
                    if (moveType == MoveType.MOVE_ANGE) {
                        _move.SetDestRotation(ange, ThisActionUnit.CurrentActionConfig.canRotate);
                    }

                    break;
            }

            ThisData.lastMoveType = moveType;
            UpdateForwardSpeed();
        }
        #endregion
        #region 功能键相关
        public void OnFunctionKeyDown(ushort keyCode) {
            switch (keyCode) {
                case InputDefine.FUNCTION_KEY1:
                    ThisActionUnit.NormalAttackKeyDown();
                    break;
            }
        }
        public void OnFunctionKeyUp(ushort keyCode) {

        }
        #endregion
        #endregion

        #region IPoolObject
        public uint ObjectIndex {
            get;
            set;
        }
        public void Release() {
            TickerManager.Instance.RemoveFixedTick(this);
            TickerManager.Instance.RemoveTick(this);
            Init();
            ThisConfig = null;
            ThisActionUnit.Release();
            ThisView.Release();
        }
        #endregion
        #region IFixedTick interface
        public void FixedUpdate(uint fixedTickCount, float delta) {
            #region 动作切换
            ThisActionUnit.Update(delta);
            #endregion

            #region 移动相关
            if (_move.HaveSpeed) {
                _move.Move(delta);
            }
            if ((_move.position - ThisView.MainTransform.position).magnitude >= 10f) {
                _move.UpdateViewPosition();
            }
            #endregion
        }
        #endregion
        #region ITick interface
        public void Update(float delta) {
            if (_move.HaveSpeed) {
                ThisView.Move(delta, _move.forwardSpeed, _move.Forward);
            }
            ThisView.UpdateRotate(delta);
        }
        #endregion

        public Vector3 Position {
            get {
                return _move.position;
            }
            set {
                _move.SetPosition(value);
            }
        }
    }
}