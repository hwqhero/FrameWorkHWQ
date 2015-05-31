using System;
using System.Collections.Generic;
using System.Text;
using NetEntityHWQ;

namespace UnitySocket.Client
{
    public class OperationData
    {
        private OperationData() { }
        private object bindObj;
        private object obj;
        private long sendTime;
        private short errorCode;
        public object BindObj
        {
            get
            {
                return bindObj;
            }
        }

        public short GetErrorCode
        {
            get
            {
                return errorCode;
            }
        }


        public string RecviceTime
        {
            get
            {
                TimeSpan ts = DateTime.Now - DateTime.FromBinary(sendTime);
                return ts.TotalMilliseconds + " ms";
            }
        }

        public static OperationData CreateByProtocol(ProtocolData pd,object bindObject)
        {
             OperationData od = new OperationData();
             od.obj = pd.baseData;
             od.sendTime = pd.sendTime;
             od.bindObj = bindObject;
             od.errorCode = pd.errorCode;
             return od;
        }

        /// <summary>
        /// 接受类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>当前类型集合</returns>
        public List<T> AsList<T>()
        {
            return obj as List<T>;
        }

        /// <summary>
        /// 对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>当前类型对象</returns>
        public T AsNetObject<T>() where T : BaseNetHWQ
        {
            return obj as T;
        }
    }
}
