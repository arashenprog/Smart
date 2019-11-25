using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
   public class ProductsSuppliersInputModel
    {
        public List<Guid> SuppliersId { set; get; }

        public List<Guid> ProductsId { set; get; }
    }
}
