using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    class DataSourceInput
    {
        public string Code { get; set; }
        public Guid ID { get; set; }
        public int SourceType { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string QueryParameters { get; set; }
        public string Order { get; set; }
        public List<QueryFilterItems> Fields { get; set; }

    }
}
