using CommonLib.Utility;
using UnityEngine;

namespace MainClient.Character {
    internal class CharacterMoveUnit {
        private Vector3 _rotation = Vector3.zero;
        private float _destRotateY = 555f;
        public Vector3 position = Vector3.zero;

        private CharacterBase _host = null;

        public float forwardSpeed;
        private const float _ROTATE_SPEED = 900f;

        private CharacterMoveUnit() {
        }
        public CharacterMoveUnit(CharacterBase host) {
            _host = host;
        }

        public Vector3 Rotation {
            get {
                return _rotation;
            }
            set {
                _rotation = value;
                SetDestRotation(_rotation.y, true);
                _host.ThisView.UpdateRotation(_rotation);
            }
        }
        public bool HaveSpeed => !CommonFunction.IsZero(forwardSpeed);

        public Vector3 Forward {
            get {
                return Quaternion.Euler(_rotation) * Vector3.forward;
            }
            set {
                float ang = CommonFunction.GetAngle(value.x, value.z);
                SetDestRotation(CommonFunction.ChangeAnge(ang), true);
                _host.ThisView.UpdateRotation(_rotation);
            }
        }

        public void Init() {
            forwardSpeed = 0f;
            _destRotateY = 555f;
            position = Vector3.zero;
            _rotation.Set(0f, 0f, 0f);
            _host.ThisView.UpdateRotation(_rotation);
        }

        public bool SetDestRotation(float destRotation, bool canRotate) {
            float desty = _destRotateY < 500f ? _destRotateY : _rotation.y;
            if (!CommonFunction.IsZero(destRotation - desty)) {
                if (canRotate) {
                    _rotation.y = destRotation;
                    _host.ThisView.SetDestRotation(destRotation, _ROTATE_SPEED);
                    _destRotateY = 555f;
                }
                else {
                    _destRotateY = destRotation;
                }
                return true;
            }
            return false;
        }

        public void Update() {
            if (_destRotateY < 500f) {
                _rotation.y = _destRotateY;
                _host.ThisView.SetDestRotation(_destRotateY, _ROTATE_SPEED);
                _destRotateY = 555f;
            }
        }

        public void Move(float delta) {
            position = CommonMoveUnit.Move(position, Forward, delta, forwardSpeed);
        }

        public void UpdateViewPosition() {
            _host.ThisView.MoveUpdate(position);
        }

        public void SetPosition(Vector3 pos) {
            position = pos;
            UpdateViewPosition();
        }
    }
}
