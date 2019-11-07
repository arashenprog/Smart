using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    public class EntityFields
    {
        public Guid ETFD_ID{ get; set; }
        public Guid ETFD_ENTT_ID{ get; set; }
        public string ETFD_NAME { get; set; }
        public string ETFD_ALIAS { get; set; }
    }
}
