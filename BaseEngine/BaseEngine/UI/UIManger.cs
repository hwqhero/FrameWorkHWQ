using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseEngine.UI
{
    /// <summary>
    /// UI管理类
    /// </summary>
    public class UIManger : MetaHWQ
    {
        internal static WindowDispatch d;

        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.BindByObject(this);
            foreach (MethodInfo mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (object attr in mi.GetCustomAttributes(typeof(MainMethodAttr), true))
                {
                    MainMethodAttr ema = attr as MainMethodAttr;
                    if (ema != null)
                    {
                        HWQEngine.entryPointEvent += (Action)Delegate.CreateDelegate(typeof(Action), this, mi, true);
                    }
                }
            }
        }
        protected WindowDispatch Dispathc
        {
            get
            {
                return d;
            }
        }
    }
}
