using UnityEngine;
using CommonLib;

namespace MainClient
{
    public class CharacterView
    {
        private SmoothMove _smoothRotate = new SmoothMove();
        private GameObject _source = null;
        private Animator _animator = null;
        private Transform _transform = null;

        public CharacterView()
        {
        }

        public Transform MainTransform
        {
            get
            {
                return _transform;
            }
        }
        public GameObject MainGameObject
        {
            get
            {
                return _source;
            }
        }
        public void StopRotate()
        {
            _smoothRotate.Stop();
        }

        public bool UpdateRotate(float detla)
        {
            float y = 0f;
            bool change = _smoothRotate.Update(detla, ref y);
            if (change)
            {
                var ang = _transform.eulerAngles;
                ang.y = y;
                _transform.eulerAngles = ang;
            }
            else
            {
                StopRotate();
            }
            return change;
        }

        public bool SetDestRotation(float destRotation, float rotateSpeed)
        {
            destRotation += 10f;
            if (destRotation != _smoothRotate.Dest || !_smoothRotate.InMoveing)
            {
                float originRotation = _transform.eulerAngles.y;
                if (originRotation - destRotation > 180f)
                {
                    originRotation -= 360f;
                }
                else if (originRotation - destRotation < -180f)
                {
                    originRotation += 360f;
                }
                _smoothRotate.SetValue(originRotation, destRotation, rotateSpeed);
                return true;
            }
            return false;
        }

        public void Init()
        {
            _smoothRotate.Stop();
        }

        public void Release()
        {
            Init();
            if (_animator != null)
            {
                _animator.speed = 1f;
                _animator.enabled = false;
            }

            if (_source != null)
            {
                _source.SetActive(false);
            }

            _animator = null;
            _transform = null;

            Object.DestroyImmediate(_source);
            _source = null;

        }

        public void ChangeView(string location, CharacterConfig config)
        {
            if (_source != null)
            {
                Object.DestroyImmediate(_source);
                _source = null;
            }

            var o = Resources.Load("Prefabs/Role/" + location);

            _source = Object.Instantiate(o) as GameObject;
            _source.SetActive(true);
            _animator = _source.GetComponent<Animator>();
            _transform = _source.GetComponent<Transform>();
        }

        protected void MoveAll(Vector3 pos)
        {
            if (_transform != null)
            {
                _transform.position = pos;
            }
        }

        public void SetCondition(int paramID, int value)
        {
            _animator.SetInteger(paramID, value);
        }

        public void UpdateRotation(Vector3 rotation)
        {
            _transform.eulerAngles = rotation;
            StopRotate();
        }

        public void MoveUpdate(Vector3 position)
        {
            MoveAll(position);
        }

        public void Move(float detla, float speed, Vector3 forward)
        {
            Vector3 pos = _transform.position;
            Vector3 to = CommonMoveUnit.Move(pos, forward, detla, speed);
            MoveAll(to);
        }

    }
}
