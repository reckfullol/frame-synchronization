using CommonLib;
using CommonLib.Utility;

namespace MainClient.Ticker {
    internal class TickerManager : Singleton<TickerManager> {
        private uint _fixedTickCount = 0;
        private delegate void UpdateDelegate(float delta);
        private delegate void LateUpdateDelegate();
        private delegate void FixedUpdateDelegate(uint fixedTickCount, float delta);

        private UpdateDelegate _ticks = null;
        private LateUpdateDelegate _lateTicks = null;
        private FixedUpdateDelegate _fixedTicks = null;

        public void Update(float delta) {
            _ticks?.Invoke(delta);
        }

        public void LateUpdate() {
            _lateTicks?.Invoke();
        }

        public void FixedUpdate() {
            ++_fixedTickCount;
            _fixedTicks?.Invoke(_fixedTickCount, CommonFunction.GAME_TIME_PRE_FRAME);
        }

        public void RemoveTick(ITick tick) {
            _ticks -= tick.Update;
        }

        public void RemoveLateTick(ILateTick tick) {
            _lateTicks -= tick.LateUpdate;
        }

        public void RemoveFixedTick(IFixedTick fixedTick) {
            _fixedTicks -= fixedTick.FixedUpdate;
        }

        public void AddTick(ITick tick) {
            _ticks += tick.Update;
        }

        public void AddLateTick(ILateTick tick) {
            _lateTicks += tick.LateUpdate;
        }

        public void AddFixedTick(IFixedTick fixedTick) {
            _fixedTicks += fixedTick.FixedUpdate;
        }

        private void ClearTicks() {
            _ticks -= _ticks;
            _lateTicks -= _lateTicks;
        }

        private void ClearFixedTicks() {
            _fixedTicks -= _fixedTicks;
        }

        public uint FiexTickCount {
            set {
                _fixedTickCount = value;
            }
        }

        public void DriveToTickCount(uint fixedTickCount) {
            while (fixedTickCount > _fixedTickCount) {
                FixedUpdate();
            }
        }

        public void ClearAllTicks() {
            ClearTicks();
            ClearFixedTicks();
        }
    }
}
