using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class CrmPartnersProducts
    {
        public CrmPartnersProducts()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public Guid PnprId { get; set; }

        public Guid PnprPrtnId { get; set; }

        public Guid PnprPrdtId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual Partners PnprCrmPartners { get; set; }

        public virtual Products PnprCrmProducts { get; set; }

        #endregion

    }
}
