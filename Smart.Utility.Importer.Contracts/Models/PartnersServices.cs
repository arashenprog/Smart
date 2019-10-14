using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class CrmPartnersServices
    {
        public CrmPartnersServices()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public Guid PnsrId { get; set; }

        public Guid PnsrPrtnId { get; set; }

        public Guid PnsrSrvsId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual Partners PnsrCrmPartners { get; set; }

        public virtual CrmServices PnsrCrmServices { get; set; }

        #endregion

    }
}
