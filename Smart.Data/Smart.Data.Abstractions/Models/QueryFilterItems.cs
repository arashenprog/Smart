namespace Smart.Data.Abstractions.Models
{
    public class QueryFilterItems
    {
        public string FieldName { get; set; }
        public string Operator { get; set; } = "Equal";
        public dynamic Value { get; set; }

       
    }

  
}
