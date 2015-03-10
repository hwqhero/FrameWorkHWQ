using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    public class FSMControl : MetaScriptableHWQ
    {

        /// <summary>
        /// 状态
        /// </summary>
        private List<FSMStateRoot> stateList = new List<FSMStateRoot>();

        /// <summary>
        /// 全局状态
        /// </summary>
        private List<FSMTransition> globalStateList = new List<FSMTransition>();

        /// <summary>
        /// 当前状态
        /// </summary>
        private FSMState curState;

        private FSMControl() { }

        /// <summary>
        /// 添加全局条件
        /// </summary>
        /// <param name="transition">过渡条件</param>
        public void AddGlobalTransition(FSMTransition transition)
        {
            if (transition != null)
            {
                if (globalStateList.Contains(transition))
                {
                    globalStateList.Add(transition);
                    globalStateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
                }
            }


        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state"></param>
        public void AddState(FSMState state)
        {
            if (state != null)
            {
                if (!stateList.Contains(state))
                {
                    if (state.IsDefault)
                        curState = state;
                    state.Contorl = this;
                    stateList.Add(state);
                    stateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
                }
            }
        }

        /// <summary>
        /// 添加一组状态机
        /// </summary>
        /// <param name="stateList">状态机组</param>
        public void AddState(params FSMState[] stateList)
        {
            foreach (FSMState s in stateList)
            {
                AddState(s);
            }
        }

        public void OnUpdate()
        {
            FSMTransition target = globalStateList.Find(tr => tr.Transition());
            if (target != null)
            {
                ConvertToState(target.ToState);
            }
            else
            {
                if (curState != null)
                {
                    curState.CheckHWQ();
                    curState.OnUpdateHWQ();
                }
            }
        }

        /// <summary>
        /// 获得当前状态
        /// </summary>
        /// <returns></returns>
        public FSMState GetCurrentState()
        {
            return curState;
        }


        internal void ConvertToState(FSMState state)
        {
            if (state == null)
                return;
            if (!stateList.Contains(state))
            {
                return;
            }
            ConvertState(state);
        }

        private void ConvertState(FSMState state)
        {
            if (state != null)
            {
                curState.ExitHWQ();
                curState = state;
                curState.EntryHWQ();
            }

        }
    }
}
