using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    /// <summary>
    /// 过渡条件
    /// </summary>
    public class FSMTransition
    {
        private int sortId = 2;
        private FSMState toState;
        private System.Func<bool> transitionMethod;
        private FSMTransition() { }

        /// <summary>
        ///顺序
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

        /// <summary>
        /// 创建一个过渡
        /// </summary>
        /// <param name="state">目标状态</param>
        /// <param name="method">过渡方法</param>
        /// <returns></returns>
        public static FSMTransition Create(FSMState state, System.Func<bool> method)
        {
            if (state != null && method != null)
            {
                return new FSMTransition()
                {
                    toState = state,
                    transitionMethod = method
                };
            }
            return null;
        }

        internal FSMState ToState
        {
            get
            {
                return toState;
            }
        }

        internal bool Transition()
        {
            if (transitionMethod != null)
                return transitionMethod();
            return false;
        }
    }
}
