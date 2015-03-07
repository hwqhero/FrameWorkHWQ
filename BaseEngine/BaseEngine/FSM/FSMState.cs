using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.FSM
{
    public abstract class FSMState
    {

        private int sortId = 2;
        private List<FSMTransition> tList = new List<FSMTransition>();
        public void AddTransition(FSMTransition t)
        {
            if (t != null && !tList.Contains(t))
            {
                tList.Add(t);
            }
        }
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
        public virtual void Entry()
        {

        }

        internal void Check()
        {
            foreach (FSMTransition t in tList)
            {
                if (t.Transition())
                {
                    break;
                }
            }
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void Exit()
        {

        }
    }
}
