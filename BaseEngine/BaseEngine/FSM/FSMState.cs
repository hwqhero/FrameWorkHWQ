using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    public abstract class FSMState : FSMStateRoot
    {
        public FSMState()
        {
            base.Init(Entry, OnUpdate, Exit);
        }
        /// <summary>
        /// 进入
        /// </summary>
        protected virtual void Entry()
        {

        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 退出
        /// </summary>
        protected virtual void Exit()
        {

        }

    
    }
}
