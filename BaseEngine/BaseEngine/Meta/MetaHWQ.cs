using System;
using System.Collections.Generic;
using System.Reflection;

namespace BaseEngine
{
    /// <summary>
    /// 根类
    /// </summary>
    public class MetaHWQ : UnityEngine.MonoBehaviour
    {
        private System.Action<UpdateData> u, l, f;
        private bool enable1, enable2, enable3;

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Awake()
        {
            MethodInfo um = GetMethodByName("CustomUpdate");
            MethodInfo lm = GetMethodByName("CustomLateUpdate");
            MethodInfo fm = GetMethodByName("CustomFixedUpdate");
            if(um!=null)
            u = (Action<UpdateData>)Delegate.CreateDelegate(typeof(Action<UpdateData>), this, um, true);
            if (lm != null)
            l = (Action<UpdateData>)Delegate.CreateDelegate(typeof(Action<UpdateData>), this, lm, true);
            if (fm != null)
            f = (Action<UpdateData>)Delegate.CreateDelegate(typeof(Action<UpdateData>), this, fm, true);
        }



        private MethodInfo GetMethodByName(string name)
        {
            return GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {
            if (!enable1 && u != null)
                HWQHandlerManager.UpdateHandler += u;
            if (!enable2 && l != null)
                HWQHandlerManager.UpdateHandler += l;
            if (!enable3 && f != null)
                HWQHandlerManager.UpdateHandler += f;
        }

        protected virtual void OnDisable()
        {
            RemoveHandler();
        }


        protected virtual void OnDestroy()
        {
            RemoveHandler();
        }

        private void RemoveHandler()
        {
            enable1 = enable2 = enable3 = false;
            if (u != null)
                HWQHandlerManager.UpdateHandler -= u;
            if (l != null)
                HWQHandlerManager.UpdateHandler -= l;
            if (f != null)
                HWQHandlerManager.UpdateHandler -= f;
        }

    }
}
