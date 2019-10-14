using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class Products
    {
        public Products()
        {
            #region Generated Constructor
            PnprCrmPartnersProducts = new HashSet<CrmPartnersProducts>();
            #endregion
        }

        #region Generated Properties
        public Guid PrdtId { get; set; }

        public string PrdtCode { get; set; }

        public string PrdtName { get; set; }

        public string PrdtType { get; set; }

        public string PrdtBrand { get; set; }

        public string PrdtCatalogue { get; set; }

        public Guid PrdtPrcaId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<CrmPartnersProducts> PnprCrmPartnersProducts { get; set; }

        public virtual CrmProductsCategory PrdtPrcaCrmProductsCategory { get; set; }

        #endregion

    }
}
