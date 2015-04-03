using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    /// <summary>
    /// 状态机控制器
    /// </summary>
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
        /// 强制转换
        /// </summary>
        private Dictionary<string, FSMForceChange> changeStateList = new Dictionary<string, FSMForceChange>();

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
                if (!globalStateList.Contains(transition))
                {
                    globalStateList.Add(transition);
                    globalStateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
                }
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
                    SortState();
                    if (state.IsDefault)
                        ConvertToState(state);
                }
            }
        }

        /// <summary>
        /// 排序状态机
        /// </summary>
        public void SortState()
        {
            stateList.Sort((x, y) => x.SortingId.CompareTo(y.SortingId));
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 重新进入当前状态机
        /// </summary>
        public void ReEntryCurrentState()
        {
            ReEntryState();
        }

        /// <summary>
        /// 强制切换当前状态
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objList"></param>
        public void ForceChange(string name, params object[] objList)
        {
            if (changeStateList.ContainsKey(name) && changeStateList[name] != null)
            {
                FSMChangeData cd = FSMChangeData.Create(objList, curState);
                if (changeStateList[name].Execute(cd))
                {
                    ConvertToState(changeStateList[name].toState);
                }
            }
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


        private void ReEntryState()
        {
            if (curState != null)
            {
                curState.EntryHWQ();
            }
        }

        private void ConvertState(FSMState state)
        {
            if (state != null)
            {
                if (curState != null)
                    curState.ExitHWQ();
                curState = state;
                curState.EntryHWQ();
            }

        }
    }
}
