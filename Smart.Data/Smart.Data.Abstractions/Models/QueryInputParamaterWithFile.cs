using System.Collections.Generic;

namespace Smart.Data.Abstractions.Models
{
    public class QueryInputParamaterWithFile
    {
        public string Code { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = -1;
        public string SearchExp { get; set; }
        public List<QueryFilterItems> Filters { get; set; } = new List<QueryFilterItems>();
        public byte[] UploadedFile { get; set; }
        //public IFile UploadedFile { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
    }


}
