namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgRigureT
    {
        
        private byte rigure_id;
        
        private uint rigure_hp;
        
        private uint rigure_attack;
        
        private uint rigure_defense;
        
        private uint rigure_attack_speed;
        
        private uint rigure_move_speed;
        
        private string rigure_skill_list;
        
        public byte RigureId
        {
            get
            {
                return this.rigure_id;
            }
            set
            {
                this.rigure_id = value;
            }
        }
        
        public uint RigureHp
        {
            get
            {
                return this.rigure_hp;
            }
            set
            {
                this.rigure_hp = value;
            }
        }
        
        public uint RigureAttack
        {
            get
            {
                return this.rigure_attack;
            }
            set
            {
                this.rigure_attack = value;
            }
        }
        
        public uint RigureDefense
        {
            get
            {
                return this.rigure_defense;
            }
            set
            {
                this.rigure_defense = value;
            }
        }
        
        public uint RigureAttackSpeed
        {
            get
            {
                return this.rigure_attack_speed;
            }
            set
            {
                this.rigure_attack_speed = value;
            }
        }
        
        public uint RigureMoveSpeed
        {
            get
            {
                return this.rigure_move_speed;
            }
            set
            {
                this.rigure_move_speed = value;
            }
        }
        
        public string RigureSkillList
        {
            get
            {
                return this.rigure_skill_list;
            }
            set
            {
                this.rigure_skill_list = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_rigure_t";
        }
    }
}
