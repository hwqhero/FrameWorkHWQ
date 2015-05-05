using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolTool.Model
{
    [Serializable]
    /// <summary>
    /// 自定义属性
    /// </summary>
    public class CustomField
    {
        public string name;
        public string type;
        public bool isList;
        public string desc;
        /// <summary>
        /// 是否是外键
        /// </summary>
        public bool isRelation;
        public List<CustomRelation> relationList;

        public override string ToString()
        {
            return "<字段名>" + name + "<类型>" + type + "<是否集合>" + isList + "<描述>" + desc;
        }
    }
}
