namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgDungeonMonsterT
    {
        
        private uint dm_id;
        
        private int dungeon_id;
        
        private uint monster_id;
        
        private float reborn_x;
        
        private float reborn_y;
        
        private float reborn_z;
        
        public uint DmId
        {
            get
            {
                return this.dm_id;
            }
            set
            {
                this.dm_id = value;
            }
        }
        
        public int DungeonId
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
        
        public uint MonsterId
        {
            get
            {
                return this.monster_id;
            }
            set
            {
                this.monster_id = value;
            }
        }
        
        public float RebornX
        {
            get
            {
                return this.reborn_x;
            }
            set
            {
                this.reborn_x = value;
            }
        }
        
        public float RebornY
        {
            get
            {
                return this.reborn_y;
            }
            set
            {
                this.reborn_y = value;
            }
        }
        
        public float RebornZ
        {
            get
            {
                return this.reborn_z;
            }
            set
            {
                this.reborn_z = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_dungeon_monster_t";
        }
    }
}
