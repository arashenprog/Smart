using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
   public class SubcontractorsServicesInputModel
    {
        public List<Guid> SubcontractorsId { set; get; }

        public List<Guid> ServicesId { set; get; }
    }
}
