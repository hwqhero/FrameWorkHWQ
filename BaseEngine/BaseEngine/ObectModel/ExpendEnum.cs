
namespace BaseEngine
{
    /// <summary>
    /// 黑板更新类型
    /// </summary>
    public enum BlackboardUpdateType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 移除
        /// </summary>
        Remove,
        /// <summary>
        /// 更新
        /// </summary>
        Modify
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// 不活动的
        /// </summary>
        Inactive,
        /// <summary>
        /// 活动的
        /// </summary>
        Running,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 错误
        /// </summary>
        Failure
    }
}
