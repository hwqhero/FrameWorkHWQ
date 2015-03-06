namespace BaseEngine
{


    public abstract class RootNode : MetaScriptableHWQ
    {
        private RootNode directeNode;
        public RootNode DirecteNode
        {
            get
            {
                return directeNode;
            }
        }
        private BehaviorControl control;
        public BehaviorControl Control
        {
            get
            {
                return control;
            }
        }

        private RootNode parentNode;
        public RootNode ParentNode
        {
            get
            {
                return parentNode;
            }
            set
            {
                parentNode = value;
            }
        }

        protected bool isStart = false;
        protected bool isPasued = false;
        protected virtual void OnPasue() { }
        protected virtual void OnResume() { }
        public virtual TaskState OnTick()
        {
            if (directeNode == null)
                return TaskState.Success;
            return directeNode.OnTick();
        }

        public bool IsPaused
        {
            get { return isPasued; }
            set
            {
                if (isPasued != value)
                {
                    isPasued = value;
                    if (isPasued)
                        OnPasue();
                    else
                        OnResume();
                }
            }
        }
    }
}
