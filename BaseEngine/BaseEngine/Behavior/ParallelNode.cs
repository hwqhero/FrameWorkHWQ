using System;
using System.Collections.Generic;

namespace BaseEngine
{
    /// <summary>
    /// 并行处理
    /// </summary>
    public sealed class ParallelNode : CompositeNode
    {
        private ParallelNode() { }

        public override TaskState OnTick()
        {
            lock (locker)
            {
                for (int i = 0, len = childrenNodes.Count; i < len; i++)
                {
                    childrenNodes[i].OnTick();
                }
                return TaskState.Running;
            }
        }
    }
}

