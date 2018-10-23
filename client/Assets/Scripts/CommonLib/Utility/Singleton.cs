using System;

namespace CommonLib.Utility {
    public abstract class BaseSingleton {
        public abstract bool Init();
        public abstract void Uninit();
    }

    public abstract class Singleton<T> : BaseSingleton where T : new() {
        protected Singleton() {
            if (null != Instance) {
                throw new ApplicationException(Instance.ToString() + @" can not be created again.");
            }
        }

        public static T Instance { get; } = new T();

        public override bool Init() {
            return true;
        }

        public override void Uninit() {
        }

    }
}
