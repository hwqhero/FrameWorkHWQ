using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    public class FSMTransition
    {
        private int sortId = 2;
        private FSMState toState;
        private System.Func<bool> transitionMethod;
        private FSMTransition() { }

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

        public bool Transition()
        {
            if (transitionMethod != null)
                return transitionMethod();
            return false;
        }
    }
}
