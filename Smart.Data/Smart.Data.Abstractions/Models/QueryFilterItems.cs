namespace Smart.Data.Abstractions.Models
{
    public class QueryFilterItems
    {
        public string FieldName { get; set; }
        public QueryFilterOperator Operator { get; set; } = QueryFilterOperator.Equal;
        public object Value { get; set; }
    }

  
}
