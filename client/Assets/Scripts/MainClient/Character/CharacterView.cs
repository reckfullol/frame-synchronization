using CommonLib.Utility;
using UnityEngine;

namespace MainClient.Character
{
    public class CharacterView
    {
        private SmoothMove _smoothRotate = new SmoothMove();
        private Animator _animator = null;

        public CharacterView()
        {
        }

        public Transform MainTransform { get; private set; } = null;

        private GameObject MainGameObject { get; set; } = null;

        private void StopRotate() {
            _smoothRotate.Stop();
        }

        public bool UpdateRotate(float detla) {
            float y = 0f;
            bool change = _smoothRotate.Update(detla, ref y);
            if (change) {
                Vector3 ang = MainTransform.eulerAngles;
                ang.y = y;
                MainTransform.eulerAngles = ang;
            } else {
                StopRotate();
            }
            return change;
        }

        public bool SetDestRotation(float destRotation, float rotateSpeed) {
            destRotation += 10f;
            if (destRotation != _smoothRotate.Dest || !_smoothRotate.InMoveing) {
                float originRotation = MainTransform.eulerAngles.y;
                if (originRotation - destRotation > 180f) {
                    originRotation -= 360f;
                } else if (originRotation - destRotation < -180f) {
                    originRotation += 360f;
                }
                _smoothRotate.SetValue(originRotation, destRotation, rotateSpeed);
                return true;
            }
            return false;
        }

        public void Init() {
            _smoothRotate.Stop();
        }

        public void Release() {
            Init();
            if (_animator != null) {
                _animator.speed = 1f;
                _animator.enabled = false;
            }

            if (MainGameObject != null) {
                MainGameObject.SetActive(false);
            }

            _animator = null;
            MainTransform = null;

            Object.DestroyImmediate(MainGameObject);
            MainGameObject = null;

        }

        public void ChangeView(string location, CharacterConfig config) {
            if (MainGameObject != null) {
                Object.DestroyImmediate(MainGameObject);
                MainGameObject = null;
            }

            Object o = Resources.Load("Prefabs/Role/" + location);

            MainGameObject = Object.Instantiate(o) as GameObject;
            if (MainGameObject != null) {
                MainGameObject.SetActive(true);
                _animator = MainGameObject.GetComponent<Animator>();
                MainTransform = MainGameObject.GetComponent<Transform>();
            }
        }

        private void MoveAll(Vector3 pos) {
            if (MainTransform != null) {
                MainTransform.position = pos;
            }
        }

        public void SetCondition(int paramID, int value) {
            _animator.SetInteger(paramID, value);
        }

        public void UpdateRotation(Vector3 rotation) {
            MainTransform.eulerAngles = rotation;
            StopRotate();
        }

        public void MoveUpdate(Vector3 position) {
            MoveAll(position);
        }

        public void Move(float detla, float speed, Vector3 forward) {
            Vector3 pos = MainTransform.position;
            Vector3 to = CommonMoveUnit.Move(pos, forward, detla, speed);
            MoveAll(to);
        }

    }
}
