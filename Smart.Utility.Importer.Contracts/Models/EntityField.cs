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

        #region Generated Properties
        public Guid EtfdId { get; set; }

        public Guid EtfdEnttId { get; set; }

        public string EtfdName { get; set; }

        #endregion

        #region Generated Relationships
        public virtual Entities EtfdEntities { get; set; }

        #endregion

    }
}
