using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class Partners
    {
        public Partners()
        {
            #region Generated Constructor
            PnprCrmPartnersProducts = new HashSet<CrmPartnersProducts>();
            PnsrCrmPartnersServices = new HashSet<CrmPartnersServices>();
            #endregion
        }

        #region Generated Properties
        public Guid PrtnId { get; set; }

        public string PrtnName { get; set; }

        public Guid PrtnCntcId { get; set; }

        public Guid PrtnType { get; set; }

        public string PrtnExtraData { get; set; }

        public string PrtnCode { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<CrmPartnersProducts> PnprCrmPartnersProducts { get; set; }

        public virtual ICollection<CrmPartnersServices> PnsrCrmPartnersServices { get; set; }

        public virtual Contacts PrtnCrmContacts { get; set; }

        public virtual CrmPartnersType PrtnTypeCrmPartnersType { get; set; }

        #endregion

    }
}
