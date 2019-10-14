using System;
using System.Collections.Generic;

namespace Crm.Data.Entities
{
    public partial class CrmProductsCategory
    {
        public CrmProductsCategory()
        {
            #region Generated Constructor
            PrdtPrcaCrmProducts = new HashSet<Products>();
            #endregion
        }

        #region Generated Properties
        public Guid PrcgId { get; set; }

        public string PrcaName { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<Products> PrdtPrcaCrmProducts { get; set; }

        #endregion

    }
}
