namespace Entity
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;
    
    
    public class RpgUserEquipT
    {
        
        private ulong equip_id;
        
        private ulong user_id;

        private ulong back_id;
        
        private uint place_type;
        
        public ulong EquipId
        {
            get
            {
                return this.equip_id;
            }
            set
            {
                this.equip_id = value;
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
        
        public uint PlaceType
        {
            get
            {
                return this.place_type;
            }
            set
            {
                this.place_type = value;
            }
        }
        
        private static string TableName()
        {
            return "rpg_user_equip_t";
        }
    }
}
