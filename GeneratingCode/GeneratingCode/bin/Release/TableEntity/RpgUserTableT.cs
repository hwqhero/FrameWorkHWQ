namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgUserTableT
    {
        
        private ulong user_id;
        
        private string user_name;
        
        private string passwrod;
        
        private bool enable;
        
        /// <summary>
        /// 用户全局标识符
        /// </summary>
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
        
        /// <summary>
        /// 注册用户名（唯一）
        /// </summary>
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
        
        /// <summary>
        /// 用户密码
        /// </summary>
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
        
        /// <summary>
        /// 是否无效
        /// </summary>
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
