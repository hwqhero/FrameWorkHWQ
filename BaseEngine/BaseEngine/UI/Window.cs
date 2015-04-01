using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace BaseEngine.UI
{
    /// <summary>
    /// 窗口基础类
    /// </summary>
    public abstract class Window : UIMeta
    {
        private List<Window> childWindow = new List<Window>();
        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            int hc = GetType().GetHashCode();
            WindowDispatch.AddWindow(hc, this);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            childWindow.Clear();
            childWindow = null;
            WindowDispatch.RemoveWindow(GetType().GetHashCode(), this);
        }

        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="wo"></param>
        protected virtual void Enter(WindowObject wo)
        {

        }

        internal void EnterHWQ(WindowObject wo)
        {
            Enter(wo);
        }

        /// <summary>
        /// 退出界面
        /// </summary>
        /// <param name="wo"></param>
        /// <returns></returns>
        protected virtual WindowDicpatchEnum Exit(WindowObject wo)
        {
            return WindowDicpatchEnum.Success;
        }

        internal WindowDicpatchEnum ExitHWQ(WindowObject wo)
        {
            return Exit(wo);
        }

        internal void DeleteSelf(bool allowDestroyingAssets)
        {
            DestroyImmediate(gameObject, allowDestroyingAssets);
        }

        /// <summary>
        /// 删除一个窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="allowDestroyingAssets">是否卸载加载资源</param>
        protected void DeleteWindow<T>(bool allowDestroyingAssets) where T : Window
        {
            T t = Get<T>();
            if (t != null)
            {
                t.DeleteSelf(allowDestroyingAssets);
            }
        }

        /// <summary>
        /// 获得窗口实例
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>窗口实例</returns>
        public static T Get<T>() where T : Window
        {
            return WindowDispatch.CheckWindow<T>();
        }

        /// <summary>
        /// 获得窗口实例,不存在侧尝试创建窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>窗口实例</returns>
        public static T Load<T>() where T : Window
        {
            return WindowDispatch.GetWindow1<T>();
        }

        /// <summary>
        /// 添加子界面
        /// </summary>
        /// <param name="w">界面</param>
        public void AddChildWindow(Window w)
        {
            if (!childWindow.Contains(w))
            {
                childWindow.Add(w);
            }
            w.SetParentUI(transform);
        }


        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        internal void SetParentUI(Transform t)
        {
            transform.SetParent(t, false);
        }

        internal void RW(WindowObject wo)
        {
            ReturnWindow(wo);
        }

        /// <summary>
        /// 返回事件
        /// </summary>
        /// <param name="wo"></param>
        protected virtual void ReturnWindow(WindowObject wo)
        {

        }


        /// <summary>
        /// 进入窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsList"></param>
        /// <returns></returns>
        protected WindowDicpatchEnum DispatchWindow<T>(params object[] paramsList) where T : Window
        {
            return UIManger.d.DispatchWindow<T>(true, paramsList);
        }

        /// <summary>
        /// 返回上一个窗口
        /// </summary>
        /// <param name="paramsList"></param>
        protected void BackWindow(params object[] paramsList)
        {
            UIManger.d.BackWindow(paramsList);
        }

        /// <summary>
        /// 进入窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="create">不存在则创建</param>
        /// <param name="paramsList"></param>
        /// <returns></returns>
        protected WindowDicpatchEnum DispatchWindow<T>(bool create, params object[] paramsList) where T : Window
        {
            return UIManger.d.DispatchWindow<T>(create, paramsList);
        } 
    }


    /// <summary>
    /// 窗口对象
    /// </summary>
    public class WindowObject
    {
        private object[] paramsObj;
        private Window lastWindow;

        /// <summary>
        /// 参数列表
        /// </summary>
        public object[] ObjList
        {
            set
            {
                paramsObj = value;
            }
            get
            {
                return paramsObj;
            }
        }


        /// <summary>
        /// 上一个窗口
        /// </summary>
        public Window LastWindow
        {
            set
            {
                lastWindow = value;
            }
            get
            {
                return lastWindow;
            }
        }

        /// <summary>
        /// 上一个窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLastWindow<T>() where T : Window
        {
            return lastWindow as T;
        }

    }

    /// <summary>
    /// 窗口调度状态
    /// </summary>
    public enum WindowDicpatchEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 退出等待
        /// </summary>
        ExitWait,
    }
}
