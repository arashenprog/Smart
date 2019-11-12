
using System.Collections.Generic;

namespace Smart.Data.Abstractions.Models
{
    public class GeneralDataInsert
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
    }
    public class GeneralInsert
    {
        public List<GeneralDataInsert> List { get; set; } = new List<GeneralDataInsert>();
    }

}