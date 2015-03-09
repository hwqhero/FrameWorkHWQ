namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgMapT
    {
        
        private uint map_id;
        
        private string map_name;
        
        private float map_reborn_x;
        
        private float map_reborn_y;
        
        private float map_reborn_z;
        
        private string map_resource_name;
        
        public uint MapId
        {
            get
            {
                return this.map_id;
            }
            set
            {
                this.map_id = value;
            }
        }
        
        public string MapName
        {
            get
            {
                return this.map_name;
            }
            set
            {
                this.map_name = value;
            }
        }
        
        public float MapRebornX
        {
            get
            {
                return this.map_reborn_x;
            }
            set
            {
                this.map_reborn_x = value;
            }
        }
        
        public float MapRebornY
        {
            get
            {
                return this.map_reborn_y;
            }
            set
            {
                this.map_reborn_y = value;
            }
        }
        
        public float MapRebornZ
        {
            get
            {
                return this.map_reborn_z;
            }
            set
            {
                this.map_reborn_z = value;
            }
        }
        
        public string MapResourceName
        {
            get
            {
                return this.map_resource_name;
            }
            set
            {
                this.map_resource_name = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_map_t";
        }
    }
}
