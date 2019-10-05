using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    public class QueryGeneratedCommand
    {
        public string Command { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

  
}
