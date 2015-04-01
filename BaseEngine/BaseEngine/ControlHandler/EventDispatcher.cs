using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseEngine
{
    /// <summary>
    /// 响应对象
    /// </summary>
    public class DispatchRespone
    {
        /// <summary>
        /// 请求参数列表
        /// </summary>
        internal object[] requestList;
        /// <summary>
        /// 返回对象
        /// </summary>
        internal object responeObj;

        /// <summary>
        /// 访问请求列表
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>不存在返回NULL</returns>
        public object this[int index]
        {
            get
            {
                if (index < 0 || index > requestList.Length)
                    return null;
                return requestList[index];
            }
        }
        /// <summary>
        /// 获得响应对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>不匹配则返回NULL</returns>
        public T GetRespone<T>()
        {
            if (responeObj is T)
            {
                return (T)responeObj;
            }
            return default(T);
        }
    }   

    /// <summary>
    /// 请求
    /// </summary>
    public class DispatchRequest
    {
        internal object[] objList;
        /// <summary>
        /// 访问请求列表
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>不存在返回NULL</returns>
        public object this[int index]
        {
            get
            {
                if (index < 0 || index > objList.Length)
                    return null;
                return objList[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetIndex<T>(int index)
        {
            if (this[index] is T)
            {
                return (T)this[index];
            }
            return default(T);
        }
    }

    internal class EventObjectHWQ
    {
        public string name;
        public Delegate d;
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
                all[name] += method;
            }
            else
            {
                all.Add(name, method);
            }
            HWQEngine.Log("绑定--->" + name);
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
                allFunc[name] += method;
            }
            else
            {
                allFunc.Add(name, method);
            }
            HWQEngine.Log("绑定--->" + name);
        }

        internal static List<EventObjectHWQ> BindByObject(object obj)
        {
            List<EventObjectHWQ> d = new List<EventObjectHWQ>();
            foreach (MethodInfo mi in obj.GetType().GetMethods(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
            {
                foreach (object attr in mi.GetCustomAttributes(typeof(ActionMehtodAttr), true))
                {
                    ActionMehtodAttr ema = attr as ActionMehtodAttr;
                    if (ema != null)
                    {
                        Action<DispatchRequest> temp = (Action<DispatchRequest>)Delegate.CreateDelegate(typeof(Action<DispatchRequest>), obj, mi, true);
                        if (string.IsNullOrEmpty(ema.name))
                        {
                            d.Add(new EventObjectHWQ() { d = temp, name = mi.Name });
                            Bind(mi.Name, temp);
                        }
                        else
                        {
                            d.Add(new EventObjectHWQ() { d = temp, name = ema.name });
                            Bind(ema.name, temp);
                        }
                    }
                }

                foreach (object attr in mi.GetCustomAttributes(typeof(FuncMehtodAttr), true))
                {
                    FuncMehtodAttr ema = attr as FuncMehtodAttr;
                    if (ema != null)
                    {
                        Func<DispatchRequest, object> temp = (Func<DispatchRequest, object>)Delegate.CreateDelegate(typeof(Func<DispatchRequest, object>), obj, mi, true);
                        if (string.IsNullOrEmpty(ema.name))
                        {
                            d.Add(new EventObjectHWQ() { d = temp, name = mi.Name });
                            BindFunc(mi.Name, temp);
                        }
                        else
                        {
                            d.Add(new EventObjectHWQ() { d = temp, name = ema.name });
                            BindFunc(ema.name, temp);
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
                    all.Remove(name);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="name">事件名</param>
        /// <param name="method">方法</param>
        public static void RemoveFunc(string name, Func<DispatchRequest, object> method)
        {
            if (allFunc.ContainsKey(name))
            {
                allFunc[name] -= method;
                if (allFunc[name] == null)
                    allFunc.Remove(name);
            }
        }


        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="name">事件名</param>
        public static void RemoveKey(string name)
        {
            all.Remove(name);
            allFunc.Remove(name);
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
                return dr;
            }
            else
            {
                HWQEngine.Log(name + "<----不存在");
            }
            return null;
        }
    }
}
