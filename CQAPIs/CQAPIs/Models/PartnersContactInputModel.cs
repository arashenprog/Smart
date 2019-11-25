using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.APIs.Module.Models
{
    public class PartnersContactInputModel
    {
        public List<Guid> SuppliersId { set; get; }

        public List<Guid> ProductsId { set; get; }
    }
}
