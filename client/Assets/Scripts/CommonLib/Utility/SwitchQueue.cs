using System.Collections.Generic;

namespace CommonLib.Utility {
    public class SwitchQueue<T> {

        private Queue<T> _mConsumeQueue;
        private Queue<T> _mProduceQueue;

        public SwitchQueue() {
            _mConsumeQueue = new Queue<T>(16);
            _mProduceQueue = new Queue<T>(16);
        }

        public SwitchQueue(int capcity) {
            _mConsumeQueue = new Queue<T>(capcity);
            _mProduceQueue = new Queue<T>(capcity);
        }

        // producer
        public void Push(T obj) {
            lock (_mProduceQueue) {
                _mProduceQueue.Enqueue(obj);
            }
        }

        // consumer.
        public T Pop() {
            return (T)_mConsumeQueue.Dequeue();
        }

        public bool Empty() {
            return 0 == _mConsumeQueue.Count;
        }

        public void Switch() {
            lock (_mProduceQueue) {
                CommonFunction.Swap(ref _mConsumeQueue, ref _mProduceQueue);
            }
        }

        public void Clear() {
            lock (_mProduceQueue) {
                _mConsumeQueue.Clear();
                _mProduceQueue.Clear();
            }
        }
    }
}
