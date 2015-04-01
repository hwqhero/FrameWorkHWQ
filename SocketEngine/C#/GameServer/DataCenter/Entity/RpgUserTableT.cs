namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    using NetEntityHWQ;
    
    
    public class RpgUserTableT
    {

        private ulong user_id;

        private string user_name;

        private string passwrod;

        private bool enable;

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

        public string UserName
        {
            get
            {
                return this.user_name;
            }
            set
            {
                this.user_name = value;
            }
        }

        public string Passwrod
        {
            get
            {
                return this.passwrod;
            }
            set
            {
                this.passwrod = value;
            }
        }

        public bool Enable
        {
            get
            {
                return this.enable;
            }
            set
            {
                this.enable = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_user_table_t";
        }
    }
}
