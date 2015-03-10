using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.UI
{
    /// <summary>
    /// 窗口调度
    /// </summary>
    public class WindowDispatch : MetaHWQ
    {
        private static Dictionary<int, Window> allWindow = new Dictionary<int, Window>();
        private static List<Window> backList = new List<Window>();
        private static Window currentOpen;
        private static WindowObject wo = new WindowObject();

        private WindowDispatch()
        {

            UIManger.d = this;
        }



        internal static void AddWindow(int hc, Window w)
        {
            allWindow.Add(hc, w);
        }

        internal static void RemoveWindow(int hc, Window window)
        {
            allWindow.Remove(hc);
            backList.Remove(window);
            if (currentOpen == window)
                currentOpen = null;
        }

        internal static T GetWindow1<T>() where T : Window
        {
            int hc = typeof(T).GetHashCode();
            if (allWindow.ContainsKey(hc))
            {
                return allWindow[hc] as T;
            }
            return default(T);
        }

        /// <summary>
        /// 删除一个窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="allowDestroyingAssets">是否卸载加载资源</param>
        public void DeleteWindow<T>(bool allowDestroyingAssets) where T : Window
        {
            T t = GetWindow1<T>();
            if (t != null)
            {
                t.DeleteSelf(allowDestroyingAssets);
            }
        }

        /// <summary>
        /// 获得窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWindow<T>() where T : Window
        {
            return GetWindow<T>();
        }

        /// <summary>
        /// 没有当前窗口则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateWindow<T>() where T : Window
        {
            int hc = typeof(T).GetHashCode();
            string windowName = typeof(T).Name;
            if (allWindow.ContainsKey(hc))
            {
                return allWindow[hc] as T;
            }
            T t1 = (T)UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<T>("Windows/" + windowName));
            UIRootHWQ root = UIRootHWQ.intance;
            if (root != null)
            {
                t1.SetParentUI(root.transform);
            }
            t1.SetActive(false);
            return t1;
        }

        /// <summary>
        /// 调度窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsList">传递参数</param>
        /// <returns></returns>
        public WindowDicpatchEnum DispatchWindow<T>(params object[] paramsList) where T : Window
        {
            return DispatchWindow<T>(true, paramsList);
        }

        /// <summary>
        /// 调度窗口
        /// </summary>
        /// <typeparam name="T">窗口</typeparam>
        /// <param name="create">没有是否创建</param>
        /// <param name="paramsList">传递参数</param>
        /// <returns></returns>
        public WindowDicpatchEnum DispatchWindow<T>(bool create, params object[] paramsList) where T : Window
        {
            T t = GetWindow1<T>();
            if (!t && create)
            {
                t = CreateWindow<T>();
            }
            if (currentOpen != t)
            {
                wo.LastWindow = currentOpen;
                wo.ObjList = paramsList;
                if (currentOpen)
                {
                    WindowDicpatchEnum tenum = currentOpen.ExitHWQ(wo);
                    if (tenum == WindowDicpatchEnum.Success)
                    {
                        if (currentOpen)
                        {
                            currentOpen.SetActive(false);
                            backList.Add(currentOpen);
                        }
                    }
                    else
                    {
                        return tenum;
                    }
                }
                if (t)
                {
                    t.SetActive(true);
                    t.EnterHWQ(wo);
                    currentOpen = t;
                }
                wo.ObjList = null;
                wo.LastWindow = null;
            }

            return WindowDicpatchEnum.Success;
        }

        /// <summary>
        /// 返回上一个窗口
        /// </summary>
        /// <param name="paramsList">传递参数</param>
        public void BackWindow(params object[] paramsList)
        {
            if (backList.Count > 0)
            {
                Window enterWindow = backList[backList.Count - 1];
                backList.RemoveAt(backList.Count - 1);
                wo.LastWindow = currentOpen;
                wo.ObjList = paramsList;
                if (currentOpen)
                {
                    currentOpen.SetActive(false);
                    currentOpen.ExitHWQ(wo);
                }
                if (enterWindow)
                {
                    enterWindow.SetActive(true);
                    enterWindow.RW(wo);
                    currentOpen = enterWindow;
                }
                wo.ObjList = null;
                wo.LastWindow = null;
            }
        }
    }
}
