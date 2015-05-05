using System;
using System.Collections.Generic;

namespace ProtocolTool.Model
{
    [Serializable]
    public class CustomClass
    {
        public string name;
        public string desc;
        public string baseName;
        public List<CustomField> fieldList;
        public List<CustomRelation> relationList;
    }
}
