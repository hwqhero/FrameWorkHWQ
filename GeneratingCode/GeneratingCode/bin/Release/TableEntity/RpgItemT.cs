namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgItemT
    {
        
        private uint item_id;
        
        private uint stack_num;
        
        private float cooldown;
        
        /// <summary>
        /// 物品唯一标示
        /// </summary>
        public uint ItemId
        {
            get
            {
                return this.item_id;
            }
            set
            {
                this.item_id = value;
            }
        }
        
        /// <summary>
        /// 叠加上限
        /// </summary>
        public uint StackNum
        {
            get
            {
                return this.stack_num;
            }
            set
            {
                this.stack_num = value;
            }
        }
        
        /// <summary>
        /// 冷却时间
        /// </summary>
        public float Cooldown
        {
            get
            {
                return this.cooldown;
            }
            set
            {
                this.cooldown = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_item_t";
        }
    }
}
