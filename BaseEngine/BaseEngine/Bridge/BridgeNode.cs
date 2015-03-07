using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine.Bridge
{

    public sealed class BridgeNode : MetaScriptableHWQ
    {
        private Dictionary<string, System.Func<BridgeSender, object>> dic = new Dictionary<string, Func<BridgeSender, object>>();
        private BridgeControl control;
        internal string jobName;
        private BridgeNode() { }

        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="commangMethod"></param>
        public void AddCommand(string commandName, System.Func<BridgeSender, object> commangMethod)
        {
            if (dic.ContainsKey(commandName))
            {
                dic[commandName] = commangMethod;
            }
            else
            {
                dic.Add(commandName, commangMethod);
            }
        }

        


        /// <summary>
        /// 分配职位
        /// </summary>
        /// <param name="job">职位名称</param>
        /// <param name="members">成员</param>
        public bool DistributionJob(string job)
        {
            if (control == null)
                return false;
            jobName = job;
            control.DistributionJob(job, this);
            return true;
        }


        internal object ExCommang(string name, BridgeSender bs)
        {
            if (dic.ContainsKey(name))
            {
                return dic[name](bs);
            }
            return null;
        }

        internal void SetControl(BridgeControl c)
        {
            control = c;
        }


        /// <summary>
        /// 部门通知
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="noticeName"></param>
        /// <param name="objList"></param>
        public void NoticeByJob(string job, string noticeName, params object[] objList)
        {
            if (control)
            {
                BridgeSender bs = BridgeSender.Create();
                bs.parList = objList;
                bs.Sender = this;
                control.NoticeByJob(job, noticeName, bs);
            }
       
        }

        private void OnDestroy()
        {
            control.ExitBridge(this);
        }
    }
}
