using System;
using System.Collections.Generic;

namespace CRM.Data.Entities
{
    public partial class EntityField
    {
        public EntityField()
        {
            #region Generated Constructor
            #endregion
        }

        
        public Guid ETFD_ID { get; set; }

        public Guid ETFD_ENTT_ID { get; set; }

        public string ETFD_NAME { get; set; }


        #region Generated Relationships
        public virtual Entities EtfdEntities { get; set; }

        #endregion

    }
}
