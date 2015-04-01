using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    /// <summary>
    /// Root状态
    /// </summary>
    public abstract class FSMStateRoot
    {
        private int sortId = 2;
        private FSMControl control;
        private List<FSMTransition> transitionList = new List<FSMTransition>();
        private Dictionary<string, FSMForceChange> changeStateList = new Dictionary<string, FSMForceChange>();
        private System.Action e, o, x;
        private bool @default;
        /// <summary>
        /// 设置成默认状态
        /// </summary>
        public void Default()
        {
            @default = true;
        }
        internal bool IsDefault
        {
            get
            {
                return @default;
            }
        }

        /// <summary>
        /// 添加过渡
        /// </summary>
        /// <param name="t">过渡</param>
        public void AddTransition(FSMTransition t)
        {
            if (t != null && !transitionList.Contains(t))
            {
                transitionList.Add(t);
                SortTransition();
            }
        }

        /// <summary>
        /// 添加过度列表
        /// </summary>
        /// <param name="tList"></param>
        public void AddTransitionRange(params FSMTransition[] tList)
        {
            foreach (FSMTransition t in tList)
            {
                AddTransition(t);
            }
        }

        /// <summary>
        /// 添加强制跳转
        /// </summary>
        /// <param name="fList"></param>
        public void AddForceChange(params FSMForceChange[] fList)
        {
            foreach (FSMForceChange fc in fList)
            {
                if (fc != null)
                {
                    if (!changeStateList.ContainsKey(fc.forceName))
                    {
                        changeStateList.Add(fc.forceName, fc);
                    }
                }
            }
        }

        /// <summary>
        /// 排序过渡条件
        /// </summary>
        public void SortTransition()
        {
            transitionList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
        }



        internal void Init(Action e, Action o, Action x)
        {
        
            this.e = e;
            this.o = o;
            this.x = x;
        }

        internal FSMControl Contorl
        {
            set
            {
                control = value;
            }
        }

        /// <summary>
        /// 顺序
        /// </summary>
        public int SortingId
        {
            set
            {
                sortId = value;
            }
            get
            {
                return sortId;
            }
        }

        internal void EntryHWQ()
        {
            if (e != null)
            {
                e();
            }
        }

        internal void OnUpdateHWQ()
        {
            if (o != null)
            {
                o();
            }
        }
        internal void ExitHWQ()
        {
            if (x != null)
            {
                x();
            }
        }

        internal void CheckHWQ()
        {
            foreach (FSMTransition t in transitionList)
            {
                if (t.Transition())
                {
                    if (control != null)
                        control.ConvertToState(t.ToState);
                    break;
                }
            }
        }

    }
}
