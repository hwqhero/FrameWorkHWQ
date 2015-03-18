using System;
using System.Collections.Generic;


namespace BaseEngine.FSM
{
    /// <summary>
    /// 强制切换状态机
    /// </summary>
    public sealed class FSMForceChange
    {

        internal string forceName;

        /// <summary>
        /// 
        /// </summary>
        internal FSMState toState;
        /// <summary>
        /// 
        /// </summary>
        private System.Func<FSMChangeData, bool> changeMethod;


        private FSMForceChange()
        {

        }

        /// <summary>
        /// 创建强制切换
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="s">目标状态</param>
        /// <param name="method">验证方法</param>
        /// <returns></returns>
        public static FSMForceChange Create(string name, FSMState s, System.Func<FSMChangeData, bool> method)
        {
            if (name != null && s != null)
            {
                return new FSMForceChange()
                {
                    forceName = name,
                    toState = s,
                    changeMethod = method
                };
            }
            return null;
        }


        internal bool Execute(FSMChangeData fcd)
        {
            if (changeMethod != null)
                return changeMethod(fcd);
            return true;
        }
    }
}


