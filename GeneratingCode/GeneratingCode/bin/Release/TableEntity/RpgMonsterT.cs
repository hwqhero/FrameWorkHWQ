namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgMonsterT
    {
        
        private uint monster_id;
        
        private uint level;
        
        private uint hp;
        
        private uint attack;
        
        private uint defense;
        
        private uint move_speed;
        
        private byte state_machine;
        
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
        
        public uint Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }
        
        public uint Hp
        {
            get
            {
                return this.hp;
            }
            set
            {
                this.hp = value;
            }
        }
        
        public uint Attack
        {
            get
            {
                return this.attack;
            }
            set
            {
                this.attack = value;
            }
        }
        
        public uint Defense
        {
            get
            {
                return this.defense;
            }
            set
            {
                this.defense = value;
            }
        }
        
        public uint MoveSpeed
        {
            get
            {
                return this.move_speed;
            }
            set
            {
                this.move_speed = value;
            }
        }
        
        public byte StateMachine
        {
            get
            {
                return this.state_machine;
            }
            set
            {
                this.state_machine = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_monster_t";
        }
    }
}
