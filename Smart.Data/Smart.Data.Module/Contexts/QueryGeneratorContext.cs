using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Newtonsoft.Json;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smart.Data.Module.Contexts
{
    public class QueryGeneratorContext : IQueryGenerator
    {
        private IData _data;
        public QueryGeneratorContext(IData data)
        {
            _data = data;
        }
        [WebApi(Route = "api/getDS", Authorized = false, Method = WebApiMethod.Post)]
        public QueryGeneratedCommand Generate(QueryInputParamater input)
        {

            DataSourceInput result = _data.Query<DataSourceInput>("SELECT Top 1 * FROM ADM.ADM_DATA_SOURCES WHERE DSRC_COD=@P1",
                new DBParam { Name = "@P1", Value = input.Code }).FirstOrDefault();
            string re = "";
            if (result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT");
                if (!string.IsNullOrEmpty(result.DSRC_JSO_FIELDS))
                {
                    dynamic json = JsonConvert.DeserializeObject(result.DSRC_JSO_FIELDS);
                    for (int i = 0; i < json.Count; i++)
                    {
                        if (i == json.Count - 1)
                        {
                            sb.AppendFormat(" {0} as {1} ", json[i].name, json[i].alias);
                        }
                        else
                        {
                            sb.AppendFormat(" {0} as {1},", json[i].name, json[i].alias);
                        }

                    }

                }
                sb.AppendFormat("FROM {0} ", result.DSRC_SRC);
                if (input.Filters != null && !string.IsNullOrEmpty(result.DSRC_QRY_PARAMS))
                {

                    Match queryparameters = Regex.Match(result.DSRC_QRY_PARAMS.ToString(), @"\@([^=<>\s\']+)");
                    for (int j = 0; j < input.Filters.Count; j++)
                    {
                        foreach (object param in queryparameters.Captures)
                        {
                            QueryFilterItems item = input.Filters[j];
                            string stringparam = param.ToString();
                            if (stringparam == item.FieldName)
                            {
                                stringparam = param.ToString().Replace("@", "");
                                //sb.AppendFormat(" WHERE {0} {1} '{2}' ", stringparam, item.Operator == QueryFilterOperator.Equal ? "=" : "", item.Value);
                                sb.AppendFormat("WHERE ");
                                sb.AppendFormat(result.DSRC_QRY_PARAMS.ToString().Replace(param.ToString(), " '" + item.Value.ToString() + "' "));
                            }

                        }

                    }

                }
                if (!string.IsNullOrEmpty(result.DSRC_ORDER))
                {
                    sb.AppendFormat(" ORDER BY {0} ", result.DSRC_ORDER);
                }


                sb.AppendFormat("OFFSET {0} ROWS ", input.Skip.ToString());


                if (input.Take != 0)
                {
                    sb.AppendFormat("FETCH NEXT {0} ROWS ONLY", input.Take.ToString());
                }
                re = sb.ToString();

            }
            return new QueryGeneratedCommand
            {
                Command = re
            };

        }
    }
}
