namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgUserBackT
    {
        
        private ulong back_id;
        
        private ulong user_id;
        
        private uint item_id;
        
        private uint grid_num;
        
        private bool is_equip;
        
        /// <summary>
        /// 背包主键
        /// </summary>
        public ulong BackId
        {
            get
            {
                return this.back_id;
            }
            set
            {
                this.back_id = value;
            }
        }
        
        /// <summary>
        /// 用户id
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
        /// 物品id
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
        /// 格子id
        /// </summary>
        public uint GridNum
        {
            get
            {
                return this.grid_num;
            }
            set
            {
                this.grid_num = value;
            }
        }
        
        /// <summary>
        /// 1表示已装备
        /// </summary>
        public bool IsEquip
        {
            get
            {
                return this.is_equip;
            }
            set
            {
                this.is_equip = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_user_back_t";
        }
    }
}
