using UnityEngine;

namespace CommonLib.Utility {
    public class SmoothMove {
        private float _speed = 90f;

        private float _origin = 0f;
        private float _needTime = 0f;
        private float _time = 0f;

        public float Dest { get; private set; } = 0f;

        public void SetValue(float origin, float dest, float speed) {
            Dest = dest;
            _origin = origin;
            float dis = Mathf.Abs(Dest - _origin);
            _speed = speed;
            _needTime = dis / _speed;
            _time = 0f;
        }

        public bool Update(float detla, ref float y) {
            if (CommonFunction.LessZero(_time - _needTime)) {
                _time += detla;
                if (CommonFunction.GreatOrEqualZero(_time - _needTime)) {
                    _time = _needTime;
                }
                y = Mathf.Lerp(_origin, Dest, _time / _needTime);
                return true;
            }
            _needTime = 0f;
            return false;
        }

        public void Stop() {
            _needTime = 0f;
        }

        public bool InMoveing => CommonFunction.GreatZero(_needTime);
    }
}
