namespace CommonLib.Interface {
    /// <summary>
    /// 自己管理的非固定时间刷新接口
    /// </summary>
    public interface ITick {
        void Update(float delta);
    }

    public interface ILateTick {
        void LateUpdate();
    }

    /// <summary>
    /// 自己管理的固定时间刷新接口
    /// </summary>
    public interface IFixedTick {
        void FixedUpdate(uint fixedTickCount, float delta);
    }
}
