using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    public class Entity
    {
        public Guid ENTT_ID { get; set; }
        public string ENTT_NAME { get; set; }
        public string ENTT_SOURCE { get; set; }
    }
}
