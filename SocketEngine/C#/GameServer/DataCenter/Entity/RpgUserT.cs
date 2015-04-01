namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgUserT
    {
        
        private ulong user_id;
        
        private ulong user_role_id;
        
        private uint user_back_limit;
        
        public ulong UserId
        {
            get
            {
                return this.user_id;
            }
            set
            {
                this.user_id = value;
            }
        }
        
        public ulong UserRoleId
        {
            get
            {
                return this.user_role_id;
            }
            set
            {
                this.user_role_id = value;
            }
        }
        
        public uint UserBackLimit
        {
            get
            {
                return this.user_back_limit;
            }
            set
            {
                this.user_back_limit = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_user_t";
        }
    }
}
