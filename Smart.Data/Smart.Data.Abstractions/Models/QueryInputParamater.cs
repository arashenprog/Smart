using System.Collections.Generic;

namespace Smart.Data.Abstractions.Models
{
    public class QueryInputParamater
    {
        public string Code { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public List<QuerySortItems> Sort { get; set; } = new List<QuerySortItems>();
        public string SearchExp { get; set; }
        public List<QueryFilterItems> Filters { get; set; } = new List<QueryFilterItems>();
        public List<string> Columns { get; set; } = new List<string>();

        public List<string> Groups { get; set; } = new List<string>();

        public List<string> GroupsKeys { get; set;} = new List<string>();
    }


}
