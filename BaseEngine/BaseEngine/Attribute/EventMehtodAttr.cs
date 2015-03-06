using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseEngine
{
    public class ActionMehtodAttr : Attribute
    {
        public string name;

        public ActionMehtodAttr()
        {

        }
    }


    public class FuncMehtodAttr : Attribute
    {
        public string name;

        public FuncMehtodAttr()
        {

        }
    }

    public class MainMethodAttr : Attribute
    {

    }
}
