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
        private List<FSMState> stateList = new List<FSMState>();

        /// <summary>
        /// 全局状态
        /// </summary>
        private List<FSMState> globalStateList = new List<FSMState>();

        /// <summary>
        /// 当前状态
        /// </summary>
        private FSMState curState;

        private FSMControl() { }


        public void AddGlobalState(FSMState state)
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

        public void AddState(FSMState state)
        {
            if (state != null)
            {
                if (!stateList.Contains(state))
                {
                    stateList.Add(state);
                    stateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
                }
            }
        }


        public void OnUpdate()
        {
            if (curState != null)
            {
                curState.Check();
                curState.OnUpdate();
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
            if (!stateList.Contains(state) && !globalStateList.Contains(state))
            {
                return;
            }
            ConvertState(state);
        }

        private void ConvertState(FSMState state)
        {
            curState.Exit();
            curState = state;
            curState.Entry();
        }


    }
}
