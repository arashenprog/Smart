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

    public class QueryInputParamater
    {
        public string Code { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = -1;
        public string SearchExp { get; set; }
        public List<QueryFilterItems> Filters { get; set; } = new List<QueryFilterItems>();
    }

    public class QueryOutputParamater
    {
        public dynamic Result { get; set; }
        public int TotalCount { get; set; } = 0;
    }



    public class QueryFilterItems
    {
        public string FieldName { get; set; }
        public QueryFilterOperator Operator { get; set; } = QueryFilterOperator.Equal;
        public object Value { get; set; }
    }

    public enum QueryFilterOperator
    {
        Equal,
        NotEqual,
        Contain,
        NotContain,
    }

  
}
