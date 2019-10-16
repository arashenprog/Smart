using System.Collections.Generic;

namespace Smart.Data.Abstractions.Models
{
    public class QueryInputParamater
    {
        public string Code { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = -1;
        public string SearchExp { get; set; }
        public List<QueryFilterItems> Filters { get; set; } = new List<QueryFilterItems>();

        public List<string> Columns { get; set; } = new List<string>();
    }

  
}
