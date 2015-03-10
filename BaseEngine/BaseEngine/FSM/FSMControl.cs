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


        public void AddGlobalState(FSMTransition state)
        {
            if (state != null)
            {
                if (globalStateList.Contains(state))
                {
                    globalStateList.Add(state);
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
                    state.Contorl = this;
                    stateList.Add(state);
                    stateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
                }
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
