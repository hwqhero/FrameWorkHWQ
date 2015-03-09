using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BaseEngine
{
    public abstract class BaseSystem : MetaHWQ
    {
        private static Dictionary<int, BaseSystem> allSystem = new Dictionary<int, BaseSystem>();
        private List<EventObjectHWQ> eventObjectList;

        internal static DataCenter instance;

        protected DataCenter DataCenter
        {
            get
            {
                return instance;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            int hc = GetType().GetHashCode();
            if (allSystem.ContainsKey(hc))
            {
                DestroyImmediate(allSystem[hc]);
            }
            eventObjectList = EventDispatcher.BindByObject(this);
            allSystem.Add(hc, this);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (EventObjectHWQ eohwq in eventObjectList)
            {
                if (eohwq.d is Action<DispatchRequest>)
                {
                    EventDispatcher.Remove(eohwq.name, eohwq.d as Action<DispatchRequest>);
                }
                else if (eohwq.d is Func<DispatchRequest, object>)
                {
                    EventDispatcher.Remove(eohwq.name, eohwq.d as Func<DispatchRequest, object>);
                }
            }
            allSystem.Remove(GetType().GetHashCode());
        }


        /// <summary>
        /// 获得其他系统的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        protected T GetOtherSystem<T>() where T : BaseSystem
        {
            int hc = typeof(T).GetHashCode();
            if (allSystem.ContainsKey(hc))
            {
                return allSystem[hc] as T;
            }
            return default(T);
        }

    }
}
