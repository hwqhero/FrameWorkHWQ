namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgDungeonT
    {
        
        private uint dungeon_id;
        
        private uint map_id;
        
        public uint DungeonId
        {
            get
            {
                return this.dungeon_id;
            }
            set
            {
                this.dungeon_id = value;
            }
        }
        
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
        
        private static string TableName()
        {
            return "rpg_dungeon_t";
        }
    }
}
