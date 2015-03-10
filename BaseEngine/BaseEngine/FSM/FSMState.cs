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
        protected virtual void Entry()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void Exit()
        {

        }

    
    }
}
