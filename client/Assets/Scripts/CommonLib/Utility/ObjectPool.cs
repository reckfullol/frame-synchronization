using System.Collections.Generic;
using CommonLib.Interface;

namespace CommonLib.Utility {
    public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : IObjectPool, new() {
        private static uint _index = 0;
        private readonly Stack<T> _stack = new Stack<T>();

        public T Get() {
            T element = _stack.Count > 0 ? _stack.Pop() : new T();
            element.ObjectIndex = ++_index;
            return element;
        }

        public void Release(ref T element) {
            if (element != null) {
                element.ObjectIndex = 0;
                element.Release();
                _stack.Push(element);
            }
        }

        public void Clear() {
            _stack.Clear();
        }

        public uint Index {
            set {
                _index = value;
            }
            get {
                return ++_index;
            }
        }
    }
}
