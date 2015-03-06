namespace BaseEngine
{
    /// <summary>
    /// 条件节点
    /// </summary>
    public sealed class ConditionNode : RootNode
    {
        public System.Func<object, bool> CheckFunc;
        public object args;

        private ConditionNode() { }
        public bool Check()
        {
            if (CheckFunc == null)
            {
                return false;
            }
            return CheckFunc(args);
        }

        public override TaskState OnTick()
        {
            if (Control == null)
            {
                return TaskState.Failure;
            }
            return Check() ? TaskState.Success : TaskState.Failure;
        }
    }
}
