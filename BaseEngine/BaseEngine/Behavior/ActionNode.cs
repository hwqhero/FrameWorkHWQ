using System;
using System.Collections.Generic;
namespace BaseEngine
{
    public sealed class ActionNode<T> : RootNode
    {
        private Func<T, TaskState> action;
        private T t;
        private ActionNode()
        {
        }

        public override TaskState OnTick()
        {
            if (action != null)
                return action(t);
            return TaskState.Success;
        }
    }
}
