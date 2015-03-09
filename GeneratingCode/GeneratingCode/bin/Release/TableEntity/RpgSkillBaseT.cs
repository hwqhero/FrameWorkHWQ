namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgSkillBaseT
    {
        
        private uint skill_id;
        
        private uint skill_lv;
        
        public uint SkillId
        {
            get
            {
                return this.skill_id;
            }
            set
            {
                this.skill_id = value;
            }
        }
        
        public uint SkillLv
        {
            get
            {
                return this.skill_lv;
            }
            set
            {
                this.skill_lv = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_skill_base_t";
        }
    }
}
