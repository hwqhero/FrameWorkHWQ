using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseEngine.UI
{
    public class UIBaseItem : MetaHWQ
    {
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
