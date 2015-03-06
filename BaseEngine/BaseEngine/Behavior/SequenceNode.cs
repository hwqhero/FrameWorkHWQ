using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BaseEngine
{
    /// <summary>
    /// 顺序
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        protected readonly List<ConditionNode> conditionNodes = new List<ConditionNode>();

        private SequenceNode() { }

        public virtual void AddCondNode(ConditionNode node)
        {
            lock (locker)
            {
                if (!conditionNodes.Contains(node))
                {
                    conditionNodes.Add(node);
                }
                node.ParentNode = this;
            }
        }

        public virtual void RemoveCondNode(RootNode node)
        {
            lock (locker)
            {
                if (!childrenNodes.Contains(node))
                {
                    childrenNodes.Remove(node);
                }
                node.ParentNode = null;
            }
        }

        public override TaskState OnTick()
        {
            lock (locker)
            {
                TaskState result = TaskState.Success;
                for (int i = 0, len = conditionNodes.Count; i < len; i++)
                {
                    TaskState state = conditionNodes[i].OnTick();
                    if (state == TaskState.Inactive)
                        continue;
                    if (state == TaskState.Failure)
                    {
                        return TaskState.Failure;
                    }
                    else if (state != TaskState.Success)
                    {
                        result = TaskState.Running;
                    }
                }

                for (int i = 0, len = childrenNodes.Count; i < len; i++)
                {
                    TaskState state = childrenNodes[i].OnTick();
                    if (state == TaskState.Inactive)
                        continue;
                    if (state == TaskState.Failure)
                    {
                        return TaskState.Failure;
                    }
                    else if (state != TaskState.Success)
                    {
                        result = TaskState.Running;
                    }
                }

                return result;
            }
        }
    }
}
