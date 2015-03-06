using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BaseEngine
{
    /// <summary>
    /// 复合节点
    /// </summary>
    public abstract class CompositeNode : RootNode
    {
        protected readonly object locker = new object();
        protected List<RootNode> childrenNodes = new List<RootNode>();
        public virtual void AddNode(RootNode node)
        {

        }

        public virtual void RemoveNode(RootNode node)
        {

        }

        public override TaskState OnTick()
        {
            lock (locker)
            {
                for (int i = 0, len = childrenNodes.Count; i < len; i++)
                {
                    switch (childrenNodes[i].OnTick())
                    {
                        case TaskState.Inactive:
                            continue;
                        case TaskState.Success:
                            return TaskState.Running;
                    }
                }
            }
            return TaskState.Failure;
        }
    }
}

