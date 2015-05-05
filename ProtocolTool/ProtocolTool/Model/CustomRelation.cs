using System;
using System.Collections.Generic;

namespace ProtocolTool.Model
{
    [Serializable]
    /// <summary>
    /// 自定义关系
    /// </summary>
    public class CustomRelation
    {
        /// <summary>
        /// 本身属性
        /// </summary>
        public string myFieldName;
        /// <summary>
        /// 目标属性
        /// </summary>
        public string targetFieldName;
        /// <summary>
        /// 符号
        /// </summary>
        public int type;
    }
}
