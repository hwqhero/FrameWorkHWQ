using ServerEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.ServerSystem
{
    public abstract class BaseSystem
    {
        private static Dictionary<int, BaseSystem> allSystem = new Dictionary<int, BaseSystem>();
        public BaseSystem()
        {
            int hc = GetType().GetHashCode();
            if (!allSystem.ContainsKey(hc))
            {
                allSystem.Add(hc, this);
                Console.WriteLine("创建系统--->" + GetType().Name);
            }
        }

        /// <summary>
        /// 获得所有用户列表
        /// </summary>
        /// <returns></returns>
        protected List<SocketUser> GetAllUser()
        {
            return SocketServer.Instance.GetUserList();
        }

        /// <summary>
        /// 查找用户列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected List<SocketUser> FindUserList(Predicate<SocketUser> predicate)
        {
      
            return SocketServer.Instance.GetUserList().FindAll(predicate);
        }

        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected SocketUser FindUserObject(Predicate<SocketUser> predicate)
        {
            return SocketServer.Instance.GetUserList().Find(predicate);
        }

        /// <summary>
        /// 获得其他系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetOtherSystem<T>() where T : BaseSystem
        {
             return GetSystem<T>();
        }

        internal static T GetSystem<T>() where T : BaseSystem
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
