using System;
using System.Collections.Generic;

namespace CRM.Data.Entities
{
    public partial class Entities
    {
        public Entities()
        {
            #region Generated Constructor
            EtfdEntityFields = new HashSet<EntityField>();
            #endregion
        }

        #region Generated Properties
        public Guid EnttId { get; set; }

        public string EnttName { get; set; }

        public string EnttSource { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<EntityField> EtfdEntityFields { get; set; }

        #endregion

    }
}
