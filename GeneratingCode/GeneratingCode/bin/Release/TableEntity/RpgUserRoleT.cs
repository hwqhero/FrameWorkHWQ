namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgUserRoleT
    {
        
        private ulong role_id;
        
        private ulong user_id;
        
        private uint role_level;
        
        private string role_name;
        
        private uint role_exp;
        
        private byte role_id_config;
        
        private float last_point_x;
        
        private float last_point_y;
        
        private float last_point_z;
        
        private uint last_map_id;
        
        /// <summary>
        /// 角色id
        /// </summary>
        public ulong RoleId
        {
            get
            {
                return this.role_id;
            }
            set
            {
                this.role_id = value;
            }
        }
        
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
        /// 角色等级
        /// </summary>
        public uint RoleLevel
        {
            get
            {
                return this.role_level;
            }
            set
            {
                this.role_level = value;
            }
        }
        
        public string RoleName
        {
            get
            {
                return this.role_name;
            }
            set
            {
                this.role_name = value;
            }
        }
        
        public uint RoleExp
        {
            get
            {
                return this.role_exp;
            }
            set
            {
                this.role_exp = value;
            }
        }
        
        public byte RoleIdConfig
        {
            get
            {
                return this.role_id_config;
            }
            set
            {
                this.role_id_config = value;
            }
        }
        
        public float LastPointX
        {
            get
            {
                return this.last_point_x;
            }
            set
            {
                this.last_point_x = value;
            }
        }
        
        public float LastPointY
        {
            get
            {
                return this.last_point_y;
            }
            set
            {
                this.last_point_y = value;
            }
        }
        
        public float LastPointZ
        {
            get
            {
                return this.last_point_z;
            }
            set
            {
                this.last_point_z = value;
            }
        }
        
        public uint LastMapId
        {
            get
            {
                return this.last_map_id;
            }
            set
            {
                this.last_map_id = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_user_role_t";
        }
    }
}
