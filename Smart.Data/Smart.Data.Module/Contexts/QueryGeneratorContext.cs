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
                sb.Append("Select");
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

                        sb.AppendLine();
                    }

                }
                sb.AppendFormat("from {0}", result.DSRC_SRC);
                if (input.Filters != null && !string.IsNullOrEmpty(result.DSRC_QRY_PARAMS))
                {

                    Match queryparameters = Regex.Match(result.DSRC_QRY_PARAMS.ToString(), @"\@([^=<>\s\']+)");
                    foreach (QueryFilterItems item in input.Filters)
                    {

                        foreach (object param in queryparameters.Captures)
                        {
                            string stringparam = param.ToString();
                            if (stringparam == item.FieldName)
                            {
                                sb.AppendFormat(" Where {0} = {1}", stringparam, item.Value);
                            }

                        }

                    }
                    sb.AppendFormat("Where ");

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
