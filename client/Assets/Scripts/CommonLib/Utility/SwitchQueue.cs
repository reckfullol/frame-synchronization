using System.Collections.Generic;

namespace CommonLib
{
    public class SwitchQueue<T>
    {

        private Queue<T> mConsumeQueue;
        private Queue<T> mProduceQueue;

        public SwitchQueue()
        {
            mConsumeQueue = new Queue<T>(16);
            mProduceQueue = new Queue<T>(16);
        }

        public SwitchQueue(int capcity)
        {
            mConsumeQueue = new Queue<T>(capcity);
            mProduceQueue = new Queue<T>(capcity);
        }

        // producer
        public void Push(T obj)
        {
            lock (mProduceQueue)
            {
                mProduceQueue.Enqueue(obj);
            }
        }

        // consumer.
        public T Pop()
        {

            return (T)mConsumeQueue.Dequeue();
        }

        public bool Empty()
        {
            return 0 == mConsumeQueue.Count;
        }

        public void Switch()
        {
            lock (mProduceQueue)
            {
                CommonFunction.Swap(ref mConsumeQueue, ref mProduceQueue);
            }
        }

        public void Clear()
        {
            lock (mProduceQueue)
            {
                mConsumeQueue.Clear();
                mProduceQueue.Clear();
            }
        }
    }
}
