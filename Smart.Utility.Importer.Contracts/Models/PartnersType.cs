using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class CrmPartnersType
    {
        public CrmPartnersType()
        {
            #region Generated Constructor
            PrtnTypeCrmPartners = new HashSet<Partners>();
            #endregion
        }

        #region Generated Properties
        public Guid PntpId { get; set; }

        public string PntpName { get; set; }

        public string PntpCode { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<Partners> PrtnTypeCrmPartners { get; set; }

        #endregion

    }
}
