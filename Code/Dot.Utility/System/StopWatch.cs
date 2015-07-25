
namespace Dot.Utility
{
    /// <summary>
    /// 计时器-计算执行的时间差-精确到微妙
    /// </summary>
    public class StopWatch
    {
        #region 构造函数

        /// <summary>
        /// Constructor
        /// </summary>
        public StopWatch()
        {
            Watch = new System.Diagnostics.Stopwatch();
            Reset();
        }

        #endregion

        #region Functions

        /// <summary>
        /// 开始计时
        /// </summary>
        public virtual void Start()
        {
            Reset();
            Watch.Start();
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public virtual void Reset()
        {
            Watch.Reset();
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        public virtual void Stop()
        {
            Watch.Stop();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// 返回计时器的总运行时间
        /// </summary>
        public virtual long ElapsedTime { get { return Watch.ElapsedMilliseconds; } }

        /// <summary>
        /// 内部计时器对象
        /// </summary>
        protected System.Diagnostics.Stopwatch Watch { get; set; }

        #endregion
    }
}