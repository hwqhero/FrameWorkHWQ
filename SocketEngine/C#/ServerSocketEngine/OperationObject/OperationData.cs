using NetEntityHWQ;
using ServerEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.OperationObject
{
    public class OperationData
    {
        private OperationData() { }
        private object obj;
        private SocketUser user;
        private long sendTime;
        private short errorCode;
        public string RecviceTime
        {
            get
            {
                TimeSpan ts = DateTime.Now - DateTime.FromBinary(sendTime);
                return ts.TotalMilliseconds + " ms";
            }
        }

        internal static OperationData Create(ProtocolData pd,SocketUser su)
        {
            OperationData od = new OperationData();
            od.obj = pd.baseData;
            od.user = su;
            od.sendTime = pd.sendTime;
            od.errorCode = pd.errorCode;
            return od;
        }


        /// <summary>
        /// 用户信息
        /// </summary>
        public SocketUser User
        {
            get
            {
                return user;
            }
        }

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <returns></returns>
        public short ErrorCode()
        {
            return errorCode;
        }


        public List<T> AsList<T>() 
        {
            return obj as List<T>;
        }

        public T NetObject<T>() where T : BaseNetHWQ
        {
            return obj as T;
        }
    }
}
