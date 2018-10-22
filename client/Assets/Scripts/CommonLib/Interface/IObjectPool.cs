
namespace CommonLib
{
    /// <summary>
    /// 对象池对象所需要的接口
    /// </summary>
    public interface IObjectPool
    {
        uint ObjectIndex
        {
            get;
            set;
        }
        void Release();
    }
}
