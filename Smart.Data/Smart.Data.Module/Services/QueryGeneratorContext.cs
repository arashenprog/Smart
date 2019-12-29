using ACoreX.Configurations.Abstractions;
using ACoreX.WebAPI;
using ACoreX.WebAPI.Abstractions;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smart.Data.Module.Services
{
    public class QueryGeneratorContext : IQueryGenerator
    {
        IConfiguration _configuration;
        public QueryGeneratorContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [WebApi(Route = "api/data/query", Authorized = false, Method = WebApiMethod.Post)]
        public dynamic Generate(QueryInputParamater input)
        {
            using (IDbConnection conn = new SqlConnection(_configuration.Get("ConnectionString:SQLConnection")))
            {
                try
                {

                    IEnumerable<dynamic> result;
                    conn.Open();
                    var args1 = new DynamicParameters();
                    args1.Add("@P1", input.Code);
                    DataSourceInput data = conn.QuerySingle<DataSourceInput>("SELECT Top 1 * FROM ADM.ADM_DATA_SOURCES WHERE DSRC_COD=@P1", args1);
                    if (string.IsNullOrEmpty(data.DSRC_JSO_FIELDS))
                        throw new InvalidOperationException("Invalid columns data");

                    if (data != null)
                    {
                        
                        var dbParams = new DynamicParameters();
                        var sql = new StringBuilder();
                        // View And Tables
                        if (data.DSRC_TYP_SRC == DataSourceType.Table || data.DSRC_TYP_SRC == DataSourceType.View)
                        {
                            //TODO: remove sql injection
                            if (input.Groups.Any() && input.GroupsKeys.Count != input.Groups.Count)
                            {
                                input.Columns[0] = "[" + input.Groups[input.GroupsKeys.Count] + "]";

                            }

                            sql.AppendFormat("SELECT {0},overall_count = COUNT(*) OVER()  FROM (SELECT ", input.Columns.Count > 0 ? String.Join(",", input.Columns) : "*");
                            var json = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(data.DSRC_JSO_FIELDS);

                            var list = json.Where(c => input.Groups.Contains((string)c.alias));
                            if (input.Groups.Any() && input.GroupsKeys.Count != input.Groups.Count)
                            {
                                sql.AppendFormat("distinct ");
                                

                                
                                foreach (var item in list)
                                {
                                   
                                    if (input.Groups[input.GroupsKeys.Count]== (string)item.alias)
                                    {
                                       var isField= input.Sort.Find(x => x.Field == (string)item.alias);

                                        if (isField == null)
                                        {
                                            input.Sort.Add(new QuerySortItems
                                            {
                                                Field = item.alias,
                                                Sort = "asc"
                                            });

                                        }
                                       
                                        sql.AppendFormat("[{0}] AS [{1}],", item.name, item.alias);

                                    }
                                   
                                }



                            }
                            else
                            {
                                foreach (var item in json)
                                {
                                    //if (input.Columns.Count > 0 && !input.Columns.Contains(String.Format("{0}", item.alias)))
                                    //    continue;
                                    sql.AppendFormat("[{0}] AS [{1}],", item.name, item.alias);
                                }
                            }

                            sql.Length--;
                            sql.AppendFormat(" FROM {0} ", data.DSRC_SRC);
                            // Query Params 
                            if (!string.IsNullOrWhiteSpace(data.DSRC_QRY_PARAMS))
                            {
                                sql.AppendFormat(" WHERE ( {0} ", data.DSRC_QRY_PARAMS);
                                var matches = Regex.Matches(data.DSRC_QRY_PARAMS, "@[a-zA-Z0-9_]+");
                                foreach (Match p in matches)
                                {
                                    var name = p.Value.Substring(1);
                                    var item = input.Filters.FirstOrDefault(c => c.FieldName.Equals(name, StringComparison.OrdinalIgnoreCase));
                                    if (item != null)
                                        dbParams.Add(p.Value, item.Value == null ? null : item.Value.ToString());
                                    else
                                    {
                                        dbParams.Add(p.Value, null);

                                    }
                                }
                                sql.Append(")");
                                foreach (QueryFilterItems q in input.Filters)
                                {
                                    if ((q.Value != null) || (q.Value == null && (q.Operator == "is-empty" || q.Operator == "is-not-empty")))
                                    {
                                        string[] jsonField = new string[3];
                                        var type = "";
                                        var Name = "";


                                        bool isNewParam = true;
                                        foreach (var db in dbParams.ParameterNames)
                                        {
                                            if (String.Equals(db, q.FieldName, StringComparison.OrdinalIgnoreCase))
                                            {
                                                isNewParam = false;
                                            }
                                        }

                                        if (isNewParam)
                                        {
                                            if (q.FieldName.Substring(0, 1) == "$")
                                            {
                                                jsonField = q.FieldName.Split('.');
                                                q.FieldName = jsonField[1];

                                            }

                                            foreach (var it in json)
                                            {
                                                if (it.alias == q.FieldName)
                                                {
                                                    type = it.type;
                                                    Name = it.name;
                                                }

                                            }
                                            switch (type)
                                            {
                                                case "string":
                                                    switch (q.Operator.ToLower())
                                                    {
                                                        case "contains":
                                                            if (q.Value.GetType().Name == "String")
                                                            {
                                                                sql.AppendFormat(" AND ( [{0}] LIKE @{0} )", Name);
                                                                dbParams.Add('@' + Name, '%' + (q.Value == null ? null : q.Value.ToString()) + '%');
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                sql.AppendFormat(" AND ( [{0}] IN ({1}) )", Name, String.Join(',', ((JArray)q.Value)
                                                                    .ToObject<List<string>>()
                                                                    .Select(c =>
                                                                        String.Format("'{0}'", c.Trim().Replace("'", "''")))
                                                                        ));
                                                                break;
                                                            }


                                                        case "end-with":
                                                            sql.AppendFormat(" AND ( [{0}] LIKE @{0} )", Name);
                                                            dbParams.Add('@' + Name, '%' + (q.Value == null ? null : q.Value.ToString()));
                                                            break;

                                                        case "equal":

                                                            sql.AppendFormat(" AND ( [{0}] LIKE @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "is-empty":
                                                            sql.AppendFormat(" AND ( [{0}] is NULL )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "is-not-empty":
                                                            sql.AppendFormat(" AND ( [{0}] IS NOT NULL )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "not-contains":
                                                            sql.AppendFormat(" AND ( [{0}] IS NOT @{0} )", Name);
                                                            dbParams.Add('@' + Name, '%' + (q.Value == null ? null : q.Value.ToString()) + '%');
                                                            break;

                                                        case "not-equal":

                                                            sql.AppendFormat(" AND ( [{0}] NOT LIKE @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "start-with":
                                                            sql.AppendFormat(" AND ( [{0}] LIKE @{0} )", Name);
                                                            dbParams.Add('@' + Name, (q.Value == null ? null : q.Value.ToString()) + '%');
                                                            break;
                                                        case "In":
                                                            //  sql.AppendFormat(" AND ( {0} IN (@{0}) )", Name);
                                                            // dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;
                                                    }

                                                    break;
                                                case "Number":
                                                    switch (q.Operator.ToLower())
                                                    {
                                                        case "equal":
                                                            sql.AppendFormat(" AND ( [{0}] = @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "not-equal":
                                                            sql.AppendFormat(" AND ( [{0}] <> @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "greater-than":
                                                            sql.AppendFormat(" AND ( [{0}] > @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "greater-than-equal":
                                                            sql.AppendFormat(" AND ( [{0}] >= @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "less-than":
                                                            sql.AppendFormat(" AND ( [{0}] < @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "less-than-equal":
                                                            sql.AppendFormat(" AND ( [{0}] <= @{0} )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;
                                                        case "is-empty":
                                                            sql.AppendFormat(" AND ( [{0}] is NULL )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "is-not-empty":
                                                            sql.AppendFormat(" AND ( [{0}] IS NOT NULL )", Name);
                                                            dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;

                                                        case "Between":
                                                            //  sql.AppendFormat(" AND ( {0} BETWEEN @1{0} AND @2{0} )", Name);
                                                            //  dbParams.Add('@'+'1'+ Name, q.Value == null ? null : q.Value.ToString());
                                                            // dbParams.Add('@' + '2' + Name, q.Value == null ? null : q.Value.ToString());

                                                            break;

                                                        case "In":
                                                            //  sql.AppendFormat(" AND ( {0} IN (@{0}) )", Name);
                                                            // dbParams.Add('@' + Name, q.Value == null ? null : q.Value.ToString());
                                                            break;
                                                    }
                                                    break;

                                                case "dateTime":
                                                case "date":
                                                case "dateTime2":
                                                    // if (q.Operator== "between")
                                                    if (String.Equals("between", q.Operator, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        DateTime v1;
                                                        DateTime v2;

                                                        DateTime.TryParse((((Newtonsoft.Json.Linq.JContainer)q.Value).First)?.ToString(), out v1);
                                                        DateTime.TryParse((((Newtonsoft.Json.Linq.JContainer)q.Value).Last)?.ToString(), out v2);

                                                        sql.AppendFormat(" AND ( [{0}] BETWEEN  @{0}1 AND @{0}2 )", Name);
                                                        dbParams.Add('@' + Name + '1', v1);
                                                        dbParams.Add('@' + Name + '2', v2);
                                                        break;


                                                    }
                                                    else
                                                    {
                                                        DateTime v1;
                                                        DateTime.TryParse((q.Value)?.ToString(), out v1);

                                                        sql.AppendFormat(" AND ( [{0}] = @{0} )", Name);
                                                        dbParams.Add('@' + Name, v1);
                                                        break;

                                                    }

                                                case "json":
                                                    switch (q.Operator.ToLower())
                                                    {
                                                        case "contains":
                                                            sql.AppendFormat(" AND (  JSON_VALUE( [{0}] , '$.{1}' ) IN ({2})    )", Name, jsonField[2], String.Join(',', ((JArray)q.Value)
                                                                    .ToObject<List<string>>()
                                                                    .Select(c =>
                                                                        String.Format("'{0}'", c.Trim().Replace("'", "''")))
                                                                        ));
                                                            break;
                                                    }
                                                    break;

                                                case "":
                                                    return "your Filter Parameters is not Defined";

                                            }

                                        }

                                    }



                                }

                            }

                            if (input.Groups.Any() && input.GroupsKeys.Count==input.Groups.Count)
                            {
                              
                                foreach (var item in list)
                                {
                                    var N = "";
                                    N = item.name;

                                    sql.AppendFormat(" AND ( [{0}] = @{0} )", item.name);
                                        int ind = input.Groups.FindIndex(a => a == (string)item.alias);
                                        dbParams.Add('@' + N, input.GroupsKeys[ind].ToString());

                                }


                            }
                            else if(input.Groups.Any() && input.GroupsKeys.Count != input.Groups.Count)
                            {
                                for (int i = 0; i < input.GroupsKeys.Count; i++)
                                {
                                    var l = list.Where(c => (string)c.alias == input.Groups[i]);

                                    foreach (var itm in l)
                                    {
                                        var N = "";
                                        N = itm.name;
                                        sql.AppendFormat(" AND ( [{0}] = @{0} )", itm.name);
                                        int ind = input.Groups.FindIndex(a => a == (string)itm.alias);
                                        dbParams.Add('@' + N, input.GroupsKeys[ind].ToString());
                                    }
                                }
                            }

                            sql.Append(") T");
                            // Where Clause
                            string orderBY = null;
                            if (input.Sort.Count > 0)
                            {
                                foreach (var item in input.Sort)
                                {
                                    orderBY = orderBY + " [" + item.Field + "] " + item.Sort + " ,";
                                }
                                orderBY= orderBY.Substring(0, orderBY.Length-1);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(data.DSRC_ORDER))
                                {
                                    orderBY = data.DSRC_ORDER;
                                }
                            }

                            //if (!string.IsNullOrWhiteSpace(data.DSRC_ORDER))
                            //{
                            //    orderBY = data.DSRC_ORDER;
                            //}

                            // PAGING
                            if (input.Take > 0)
                            {
                                if (!string.IsNullOrEmpty(orderBY))
                                {
                                    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                                    sql.AppendFormat("OFFSET ({0}) ROWS ", input.Skip.GetValueOrDefault(0));
                                    sql.AppendFormat("FETCH NEXT {0} ROWS ONLY", input.Take);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(orderBY))
                                    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                            }

                            result = conn.Query<dynamic>(sql.ToString(), dbParams);
                            long totalCount = result.LongCount() > 0 ? result.Select(x => x.overall_count).ToList()[0] : 0;

                            //(IDictionary<String, Object>)result).Remove("Name");
                            //return result;



                            return new
                            {
                                items = result,
                                totalCount
                            };
                        }
                    }
                    throw new InvalidOperationException();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public object GetNewType(object obj)
        {
            var newprops = obj.GetType().GetProperties().Where(w => w.Name != "overall_count").ToList();
            var newType = new ExpandoObject();
            foreach (var property in newprops)
            {
                newType.TryAdd(property.Name, property.GetValue(obj));
            }
            return newType;
        }

    }
}

