using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine
{
    /// <summary>
    /// 选择节点
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        public override TaskState OnTick()
        {
            lock (locker)
            {
                for (int i = 0, len = childrenNodes.Count; i < len; i++)
                {
                    var state = childrenNodes[i].OnTick();
                    if (state == TaskState.Failure)
                        continue;
                    return state;
                }
                return TaskState.Failure;
            }

        }
    }
}

