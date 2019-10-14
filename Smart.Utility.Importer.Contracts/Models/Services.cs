using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class CrmServices
    {
        public CrmServices()
        {
            #region Generated Constructor
            PnsrCrmPartnersServices = new HashSet<CrmPartnersServices>();
            #endregion
        }

        #region Generated Properties
        public Guid SrvsId { get; set; }

        public string SrvsName { get; set; }

        public string SrvsCode { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<CrmPartnersServices> PnsrCrmPartnersServices { get; set; }

        #endregion

    }
}
