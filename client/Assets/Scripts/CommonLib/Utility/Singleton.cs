using System;

namespace CommonLib
{
    public abstract class BaseSingleton
    {
        public abstract bool Init();
        public abstract void Uninit();
    }

    public abstract class Singleton<T> : BaseSingleton where T : new()
    {
        private static readonly T _instance = new T();

        protected Singleton()
        {
            if (null != _instance)
            {
                throw new ApplicationException(_instance.ToString() + @" can not be created again.");
            }
        }

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        public override bool Init()
        {
            return true;
        }

        public override void Uninit()
        {
        }

    }
}
