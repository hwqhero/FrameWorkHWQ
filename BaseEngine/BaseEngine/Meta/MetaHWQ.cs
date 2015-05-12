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
        private List<EventObjectHWQ> eventObjectList;
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        /// <summary>
        /// 销毁回调
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (eventObjectList == null)
                return;
            foreach (EventObjectHWQ eohwq in eventObjectList)
            {
                if (eohwq.d is Action<DispatchRequest>)
                {
                    EventDispatcher.Remove(eohwq.name, eohwq.d as Action<DispatchRequest>);
                }
                else if (eohwq.d is Func<DispatchRequest, object>)
                {
                    EventDispatcher.RemoveFunc(eohwq.name, eohwq.d as Func<DispatchRequest, object>);
                }
            }
        }

        /// <summary>
        /// 绑定方法
        /// </summary>
        protected void BindObjectMethod()
        {
            eventObjectList = EventDispatcher.BindByObject(this);
        }


    }
}
