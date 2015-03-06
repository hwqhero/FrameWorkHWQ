using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseEngine
{

    public class DispatchRespone
    {
        public object[] requestList;
        public object responeObj;
    }

    public class DispatchRequest
    {
        public object[] objList;
    }

    /// <summary>
    /// 事件调度
    /// </summary>
    public class EventDispatcher
    {
        private static Dictionary<string, Action<DispatchRequest>> all = new Dictionary<string, Action<DispatchRequest>>();
        private static Dictionary<string, Func<DispatchRequest, object>> allFunc = new Dictionary<string, Func<DispatchRequest, object>>();


        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="method">方法</param>
        public static void Bind(string name, Action<DispatchRequest> method)
        {
            if (all.ContainsKey(name))
            {
                throw new Exception() { Source = "存在" };
            }
            all.Add(name, method);
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="method">方法</param>
        public static void BindFunc(string name, Func<DispatchRequest, object> method)
        {
            if (allFunc.ContainsKey(name))
            {
                throw new Exception() { Source = "存在" };
            }
            allFunc.Add(name, method);
        }
        
        internal static List<Delegate> BindByObject(object obj)
        {
            List<Delegate> d = new List<Delegate>();
            foreach (MethodInfo mi in obj.GetType().GetMethods(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
            {
                foreach (object attr in mi.GetCustomAttributes(typeof(ActionMehtodAttr), true))
                {
                    ActionMehtodAttr ema = attr as ActionMehtodAttr;
                    if (ema != null)
                    {
                        if (string.IsNullOrEmpty(ema.name))
                        {
                            Bind(mi.Name, (Action<DispatchRequest>)Delegate.CreateDelegate(typeof(Action<DispatchRequest>), obj, mi, true));
                        }
                        else
                        {
                            Bind(ema.name, (Action<DispatchRequest>)Delegate.CreateDelegate(typeof(Action<DispatchRequest>), obj, mi, true));
                        }
                    }
                }

                foreach (object attr in mi.GetCustomAttributes(typeof(FuncMehtodAttr), true))
                {
                    FuncMehtodAttr ema = attr as FuncMehtodAttr;
                    if (ema != null)
                    {
                        if (string.IsNullOrEmpty(ema.name))
                        {
                            BindFunc(mi.Name, (Func<DispatchRequest, object>)Delegate.CreateDelegate(typeof(Func<DispatchRequest, object>), obj, mi, true));
                        }
                        else
                        {
                            BindFunc(ema.name, (Func<DispatchRequest, object>)Delegate.CreateDelegate(typeof(Func<DispatchRequest, object>), obj, mi, true));
                        }
                    }
                }
            }
            return d;
        }

        /// <summary>
        /// 绑定事件  存在相同事件名   侧追加 
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="method">方法</param>
        public static void AddAndBind(string name, Action<DispatchRequest> method)
        {
            if (all.ContainsKey(name))
            {
                all[name] += method;
            }
            else
            {
                all.Add(name, method);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="method">方法</param>
        public static void Remove(string name, Action<DispatchRequest> method)
        {
            if (all.ContainsKey(name))
            {
                all[name] -= method;
                if (all[name] == null)
                    RemoveKey(name);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="name">事件名</param>
        public static void RemoveKey(string name)
        {
            all.Remove(name);
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public static void ClearEvent()
        {
            all.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="paramsList">参数列表</param>
        public static void Upward(string name, params object[] paramsList)
        {
            if (all.ContainsKey(name))
            {
                all[name](new DispatchRequest()
                {
                    objList = paramsList
                });
            }
            else
            {
                HWQEngine.Log(name + "<----不存在");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="paramsList">参数列表</param>
        /// <returns></returns>
        public static DispatchRespone UpwardAndResponse(string name, params object[] paramsList)
        {
            if (allFunc.ContainsKey(name))
            {
                DispatchRespone dr = new DispatchRespone();
                dr.requestList = paramsList;
                dr.responeObj = allFunc[name](new DispatchRequest()
                {
                    objList = paramsList
                });
            }
            else
            {
                HWQEngine.Log(name + "<----不存在");
            }
            return null;
        }
    }
}
