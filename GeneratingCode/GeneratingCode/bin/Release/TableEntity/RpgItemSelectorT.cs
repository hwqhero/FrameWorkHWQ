namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgItemSelectorT
    {
        
        private uint selector_id;
        
        private uint item_id;
        
        private byte selector_type;
        
        /// <summary>
        /// 选择器唯一标识
        /// </summary>
        public uint SelectorId
        {
            get
            {
                return this.selector_id;
            }
            set
            {
                this.selector_id = value;
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
        /// 选择器类型(1=自己,2=队伍)
        /// </summary>
        public byte SelectorType
        {
            get
            {
                return this.selector_type;
            }
            set
            {
                this.selector_type = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_item_selector_t";
        }
    }
}
