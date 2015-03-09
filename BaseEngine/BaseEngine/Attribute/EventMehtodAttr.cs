using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine
{
    public class ActionMehtodAttr : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string name;

        public ActionMehtodAttr()
        {

        }

        public ActionMehtodAttr(string n)
        {
            name = n;
        }
    }


    public class FuncMehtodAttr : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string name;

        public FuncMehtodAttr()
        {

        }

        public FuncMehtodAttr(string n)
        {
            name = n;
        }
    }

    /// <summary>
    /// 标识为 主入口
    /// </summary>
    public class MainMethodAttr : Attribute
    {

    }
}
