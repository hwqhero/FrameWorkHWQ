using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using ServerEngine.OperationObject;
using ServerEngine.Tool;
using ServerEngine.ServerSystem;

namespace ServerEngine.Core
{
    /// <summary>
    /// 服务器
    /// </summary>
    public sealed class SocketServer
    {
        internal static SocketServer Instance;
        /// <summary>
        /// 启动时间
        /// </summary>
        internal static DateTime startTime;
 
        /// <summary>
        /// 用户列表
        /// </summary>
        private List<SocketUser> m_userList = new List<SocketUser>();
        private Socket server;
        /// <summary>
        /// 保存命令操作
        /// </summary>
        private static Dictionary<byte, Dictionary<byte, Action<IProtocol, SocketUser>>> operationDic = new Dictionary<byte, Dictionary<byte, Action<IProtocol, SocketUser>>>();
        /// <summary>
        /// 当前连接数
        /// </summary>
        private int m_numConnectedSockets;

        /// <summary>
        /// 用户上线事件
        /// </summary>
        public event Action<SocketUser> connectUser;
        private Func<IProtocol> createProtocol;
        /// <summary>
        /// 用户断开连接
        /// </summary>
        public event Action<SocketUser> disconnectEvent;

        private SocketServer()
        {

        }

        public static SocketServer CreateServer()
        {
            if (Instance != null)
                return Instance;
            Instance = new SocketServer();
            ServerTimeTool.Create();
            return Instance;
        }

        public void OperationUserList(System.Action<List<SocketUser>> operationAction)
        {
            if (operationAction != null)
            {
                operationAction(m_userList);
            }
        }

        internal List<SocketUser> GetUserList()
        {
            lock (m_userList)
                return m_userList;
        }


        internal static void BeginOperation(SocketUser su, IProtocol pd)
        {
            if (operationDic.ContainsKey(pd.GetMainCMD()))
            {
                if (operationDic[pd.GetMainCMD()].ContainsKey(pd.GetSubCMD()))
                {
                    operationDic[pd.GetMainCMD()][pd.GetSubCMD()](pd, su);
                    return;
                }
            }
            Console.WriteLine("没有找到对应的解析");
        }

        public void Start(string ip, int port)
        {

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//使用 ip4 协议   ,   可靠双向的字节流 ,     tcp控制协议
            try
            {
                server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            server.Listen(8000);
            startTime = DateTime.Now;
            Console.WriteLine("服务器启动成功监听端口-->" + port);
            Assembly a = Assembly.GetCallingAssembly();
            foreach (Type t in a.GetTypes())
            {
                if (t.BaseType == typeof(BaseSystem))
                {
                    BaseSystem bo = (BaseSystem)Activator.CreateInstance(t);
                    foreach (MethodInfo mi in t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    {
                        SystemCMDAttr sa = mi.GetCustomAttribute(typeof(SystemCMDAttr)) as SystemCMDAttr;
                        if (sa != null)
                        {
                            if (operationDic.ContainsKey(sa.m))
                            {
                                if (!operationDic[sa.m].ContainsKey(sa.s))
                                {
                                    operationDic[sa.m].Add(sa.s, (Action<IProtocol, SocketUser>)mi.CreateDelegate(typeof(Action<IProtocol, SocketUser>), bo));
                                }
                            }
                            else
                            {
                                operationDic.Add(sa.m, new Dictionary<byte, Action<IProtocol, SocketUser>>());
                                operationDic[sa.m].Add(sa.s, (Action<IProtocol, SocketUser>)mi.CreateDelegate(typeof(Action<IProtocol, SocketUser>), bo));
                            }
                        }
                    }
                }
            }
   
            Accept(null);
        }

        private void Accept(SocketAsyncEventArgs accept)
        {
            if (accept == null)
            {
                accept = new SocketAsyncEventArgs();
                accept.Completed += AcceptEventArg_Completed;
            }
            else
            {
                accept.AcceptSocket = null;
            }
            try
            {
                server.AcceptAsync(accept);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs args)
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            IPEndPoint ipep = (IPEndPoint)args.AcceptSocket.RemoteEndPoint;
            SocketUser user = new SocketUser(m_numConnectedSockets, args, CloseSocketUser, IO_C, CreateProtocol());
            if(connectUser!=null)
            connectUser(user);
            lock (m_userList)
            {
                m_userList.Add(user);
            }
            Console.WriteLine("客户端->" + ipep + "上线<--->" + user.GetIPCode() + "<当前用户---->" + m_userList.Count);
            Accept(args);

        }

        public void BindProtocol(Func<IProtocol> protocol)
        {
            createProtocol = protocol;
        }

        public IProtocol CreateProtocol()
        {
            if (createProtocol != null)
                return createProtocol();
            return null;
        }

        private void IO_C(object sender, SocketAsyncEventArgs e)
        {
            lock (e)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:

                        SocketUser su = e.UserToken as SocketUser;
                        //Console.WriteLine(su.id + "<-->" + id + "<-->" + (e.AcceptSocket == su.socket));
                        su.Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        break;
                }
            }
        }

        public T GetSystem<T>() where T :BaseSystem
        {
            return BaseSystem.GetSystem<T>();
        } 


        private void CloseSocketUser(SocketUser user)
        {
            lock (m_userList)
            {
                if (m_userList.Remove(user))
                {
                    user.Clear();
                    Console.WriteLine(user.GetPoint().Address + "<--->下线<--->当前剩余用户---->" + m_userList.Count);
                }
            }
        }
    }
}
