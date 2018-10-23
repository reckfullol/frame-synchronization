using System;
using System.Collections.Generic;
using CommonLib;
using CommonLib.Utility;
using MainClient.GameSync;
using UnityEngine;

namespace MainClient.GameInput {
    internal class InputHandle {
        private static Dictionary<uint, float> _dir2RotateAng = null;

        public InputHandle() {
            if (_dir2RotateAng == null) {
                _dir2RotateAng = new Dictionary<uint, float> {
                    { InputDefine.MOVE_UP, 0f },
                    { InputDefine.MOVE_UP | InputDefine.MOVE_RIGHT, 45f },
                    { InputDefine.MOVE_RIGHT, 90f },
                    { InputDefine.MOVE_DOWN | InputDefine.MOVE_RIGHT, 135f },
                    { InputDefine.MOVE_DOWN, -180f },
                    { InputDefine.MOVE_DOWN | InputDefine.MOVE_LEFT, -135f },
                    { InputDefine.MOVE_LEFT, -90f },
                    { InputDefine.MOVE_UP | InputDefine.MOVE_LEFT, -45f }
                };
            }
        }

        private IInputListener _listener = null;
        private uint _directionKey = 0;
        public void AddInputListener(IInputListener listener) {
            _listener = listener;
        }

        public void Update(float delta) {
            if (Input.GetKeyDown(KeyCode.A)) {
                DirectionKeyDown(InputDefine.MOVE_LEFT);
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                DirectionKeyDown(InputDefine.MOVE_RIGHT);
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                DirectionKeyDown(InputDefine.MOVE_UP);
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                DirectionKeyDown(InputDefine.MOVE_DOWN);
            }

            if (Input.GetKeyUp(KeyCode.A)) {
                DirectionKeyUp(InputDefine.MOVE_LEFT);
            }
            if (Input.GetKeyUp(KeyCode.D)) {
                DirectionKeyUp(InputDefine.MOVE_RIGHT);
            }
            if (Input.GetKeyUp(KeyCode.W)) {
                DirectionKeyUp(InputDefine.MOVE_UP);
            }
            if (Input.GetKeyUp(KeyCode.S)) {
                DirectionKeyUp(InputDefine.MOVE_DOWN);
            }




            if (Input.GetKeyDown(KeyCode.J)) {
                FunctionKeyDown(InputDefine.FUNCTION_KEY1);
            }
        }

        private void OnMove(uint lastKey, uint newKey) {
            if (lastKey == newKey) {
                return;
            }
            if (newKey == InputDefine.STOP) {
                DirctionTouch(0f, MoveType.STOP);
            } else {
                DirctionTouch(_dir2RotateAng[newKey], MoveType.MOVE_ANGE);
            }
        }
        private void DirectionKeyUp(uint keyCode) {
            uint lastKey = _directionKey;
            _directionKey &= ~keyCode;
            OnMove(lastKey, _directionKey);
        }
        private void DirectionKeyDown(uint keyCode) {
            uint lastKey = _directionKey;
            switch (keyCode) {
                // 抬起反向按键
                case InputDefine.MOVE_DOWN:
                    _directionKey &= ~InputDefine.MOVE_UP;
                    break;
                case InputDefine.MOVE_UP:
                    _directionKey &= ~InputDefine.MOVE_DOWN;
                    break;
                case InputDefine.MOVE_LEFT:
                    _directionKey &= ~InputDefine.MOVE_RIGHT;
                    break;
                case InputDefine.MOVE_RIGHT:
                    _directionKey &= ~InputDefine.MOVE_LEFT;
                    break;
            }

            _directionKey |= keyCode;
            OnMove(lastKey, _directionKey);
        }

        public void DirctionTouch(float ang, MoveType moveType) {
            if (GameSyncManager.Instance.IsStepLockMode) {
                if (CommonFunction.LessZero(ang)) {
                    ang += 360f;
                }
                GameSyncManager.Instance.AddKeyInfo(InputDefine.DIRCTION_KEY, 
                    (ushort)Mathf.FloorToInt(ang + CommonFunction.EPS), 
                    (byte)moveType);
                return;
            }

            if (_listener != null)
            {
                _listener.OnMove(ang, moveType);
            }
        }

        public void FunctionKeyDown(UInt16 key)
        {
            if (GameSyncManager.Instance.IsStepLockMode)
            {
                GameSyncManager.Instance.AddKeyInfo(key, 0, 1);
                return;
            }
            if (_listener != null)
            {
                _listener.OnFunctionKeyDown(key);
            }
        }
        public void FunctionKeyUp(UInt16 key)
        {
            /*
            if (GameSyncManager.Instance.IsStepLockMode)
            {
                GameSyncManager.Instance.AddKeyInfo(key, false);
                return;
            }
            */
        }
    }
}
