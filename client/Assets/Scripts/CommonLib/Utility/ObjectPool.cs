using System.Collections.Generic;

namespace CommonLib
{
    public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : IObjectPool, new()
    {
        private static uint _INDEX = 0;
        private readonly Stack<T> stack = new Stack<T>();

        public T Get()
        {
            T element;
            if (stack.Count > 0)
            {
                element = stack.Pop();
            }
            else
            {
                element = new T();
            }
            element.ObjectIndex = ++_INDEX;
            return element;
        }

        public void Release(ref T element)
        {
            if (element != null)
            {
                element.ObjectIndex = 0;
                element.Release();
                stack.Push(element);
            }
        }

        public void Clear()
        {
            stack.Clear();
        }

        public uint Index
        {
            set
            {
                _INDEX = value;
            }
            get
            {
                return ++_INDEX;
            }
        }
    }
}
