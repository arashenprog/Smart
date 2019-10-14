using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class Contacts
    {
        public Contacts()
        {
            #region Generated Constructor
            PrtnCrmPartners = new HashSet<Partners>();
            #endregion
        }

        #region Generated Properties
        public Guid CntcId { get; set; }

        public string CntcFirstName { get; set; }

        public string CntcLastName { get; set; }

        public string CntcFullName { get; set; }

        public string CntcRole { get; set; }

        public string CntcMobile { get; set; }

        public string CntcPhone { get; set; }

        public string CntcRepEmail { get; set; }

        public string CntcOrderEmail { get; set; }

        public string CntcAddress { get; set; }

        public string CntcType { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<Partners> PrtnCrmPartners { get; set; }

        #endregion

    }
}
