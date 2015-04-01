using UnityEngine;
using System.Collections;

namespace BaseEngine
{
    /// <summary>
    /// 协程工具类
    /// </summary>
    public sealed class AsyncOperationTool : MetaHWQ
    {
        private static AsyncOperationTool tool;
        private AsyncOperationTool() { }
        private new void Awake()
        {
            tool = this;
        }

        private void AsyncCoroutine(IEnumerator routine)
        {
            StartCoroutine_Auto(routine);
        }

        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <param name="routine"></param>
        public static void StartAsync(IEnumerator routine)
        {
            tool.AsyncCoroutine(routine);
        }
    }
}
