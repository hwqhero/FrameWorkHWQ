using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BaseEngine.UI
{
    public class UIBaseItem : MetaHWQ
    {

        public UnityEvent<bool> pressEvent;

        private Transform myTf;
        protected Transform MyTF
        {
            get
            {
                if (myTf == null)
                {
                    myTf = transform;
                }
                return myTf;
            }
        }
    }
}
