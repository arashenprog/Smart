using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Newtonsoft.Json;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        [WebApi(Route = "api/data/query", Authorized = false, Method = WebApiMethod.Post)]
        public dynamic Generate(QueryInputParamater input)
        {
            string re = "";
            List<DBParam> dbparam = new List<DBParam>();
            IEnumerable<dynamic> result;
            using (SqlConnection cnn = _data.OpenConnection())
            {
                try
                {
                    DataSourceInput data = _data.Query<DataSourceInput>("SELECT Top 1 * FROM ADM.ADM_DATA_SOURCES WHERE DSRC_COD=@P1",
                    new DBParam { Name = "@P1", Value = input.Code }).FirstOrDefault();

                    if (data != null)
                    {
                        var sql = new StringBuilder();
                        // View And Tables
                        if (data.DSRC_TYP_SRC == DataSourceType.Table || data.DSRC_TYP_SRC == DataSourceType.View || data.DSRC_TYP_SRC == DataSourceType.Function)
                        {
                            //TODO: remove sql injection
                            if (input.Take > 0 && input.Skip != -1 || input.Take == null)
                                sql.AppendFormat("SELECT {0}  FROM (SELECT ", input.Columns.Count > 0 ? String.Join(",", input.Columns) : "*");
                            else
                                sql.AppendFormat("SELECT {0} {1}  FROM (SELECT ", input.Take == -1 ? "" : "TOP" + input.Take, input.Columns.Count > 0 ? String.Join(",", input.Columns) : "*");

                            if (!string.IsNullOrEmpty(data.DSRC_JSO_FIELDS))
                            {
                                dynamic json = JsonConvert.DeserializeObject(data.DSRC_JSO_FIELDS);
                                //for (int i = 0; i < json.Count; i++)
                                //{
                                //    if (i == json.Count - 1)
                                //    {
                                //        sb.AppendFormat(" {0} as {1} ", json[i].name, json[i].alias);
                                //    }
                                //    else
                                //    {
                                //        sb.AppendFormat(" {0} as {1},", json[i].name, json[i].alias);
                                //    }

                                //}
                                foreach (var item in json)
                                {
                                    //if (input.Columns.Count > 0 && !input.Columns.Contains(String.Format("{0}", item.alias)))
                                    //    continue;
                                    sql.AppendFormat("[{0}] AS [{1}],", item.name, item.alias);
                                }
                                sql.Length--;
                                if (data.DSRC_TYP_SRC == DataSourceType.Function)
                                {
                                    //sql.AppendFormat(" FROM {0} ( ", data.DSRC_SRC);
                                    //if (data.DSRC_QRY_PARAMS.Any())
                                    //{
                                    //    foreach (var item in data.DSRC_QRY_PARAMS)
                                    //    {
                                    //        sql.Append($"@{item.Name},");
                                    //        parameters.Add(new SqlParameter(item.Name, item.Value == null ? DBNull.Value : item.Value));
                                    //    }
                                    //}
                                    //sql.Length--;
                                    //sql.Append(")");
                                }
                                else
                                {
                                    sql.AppendFormat(" FROM {0} ", data.DSRC_SRC);
                                    // Query Params 
                                    if (!string.IsNullOrWhiteSpace(data.DSRC_QRY_PARAMS))
                                    {
                                        sql.AppendFormat(" WHERE {0} ", data.DSRC_QRY_PARAMS);
                                        var matches = Regex.Matches(data.DSRC_QRY_PARAMS, "@[a-zA-Z0-9_]+");
                                        foreach (Match p in matches)
                                        {
                                            var name = p.Value.Substring(1);
                                            //if (input.Filters.Any(c => c.FieldName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                                            //    continue;
                                            var item = input.Filters.FirstOrDefault(c => c.FieldName.Equals(name, StringComparison.OrdinalIgnoreCase));
                                            if (item != null)
                                                dbparam.Add(new DBParam { Name = p.Value, Value = item.Value == null ? null : item.Value.ToString() });
                                            else
                                                dbparam.Add(new DBParam { Name = p.Value, Value = null, IsNullable = true });
                                        }
                                    }
                                }
                                sql.Append(") T");
                                // Where Clause
                                // PrepareWhereClause(data, sql, data, parameters);
                                // ORDER BY
                                string orderBY = null;
                                //if (!string.IsNullOrWhiteSpace(data.DSRC_ORDER))
                                //orderBY = FixSortExpr(data.Sort);
                                //else
                                if (!string.IsNullOrWhiteSpace(data.DSRC_ORDER))
                                {
                                    orderBY = $"[{data.DSRC_ORDER}]";
                                }
                                //else if (data.ADM_DYNAMIC_FORM_ITEMs.Any())
                                //{
                                //    orderBY = $"[{data.ADM_DYNAMIC_FORM_ITEMs.OrderBy(c => c.DFMI_SEQ).First().DFMI_NAM_DISPLAY}]";
                                //}
                                // PAGING
                                long totalCount = 0;
                                if (input.Take > 0 && input.Skip != -1)
                                {
                                    totalCount = _data.Query<long>(String.Format("SELECT COUNT(*) FROM ({0}) X", sql.ToString()), dbparam.ToArray()).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(orderBY))
                                    {
                                        sql.AppendFormat(" ORDER BY {0} ", orderBY);
                                        sql.AppendFormat("OFFSET ({0}) ROWS ", input.Skip);
                                        sql.AppendFormat("FETCH NEXT {0} ROWS ONLY", input.Take);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(orderBY))
                                        sql.AppendFormat(" ORDER BY {0} ", orderBY);
                                }
                                re = sql.ToString();
                                //
                                //var sqlData = _data.Query<dynamic>(sql.ToString(), dbparam.ToArray()).ToObjectList();
                                //result.TotalRowCount = totalCount > 0 ? totalCount : sqlData.LongCount();
                                //result.Data = sqlData;
                            }
                            // Enum Types
                            //if (data.DSRC_TYP_SRC == ((int)DataSourceType.Enum))
                            //{

                            //    result.Fields.Add(new DynamicFormItem
                            //    {
                            //        Name = "ID",
                            //        Title = StringManager.GetString("PUB_VALUE"),
                            //        Order = 0,
                            //        Visible = true,
                            //        IsValue = true,
                            //        ControlType = FormControlType.TextBox,
                            //        DataType = FormItemDataType.String,
                            //    });

                            //    result.Fields.Add(new DynamicFormItem
                            //    {
                            //        Name = "Title",
                            //        Title = StringManager.GetString("PUB_TITLE"),
                            //        Order = 1,
                            //        IsDisplay = true,
                            //        Visible = true,
                            //        ControlType = FormControlType.TextBox,
                            //        DataType = FormItemDataType.String,
                            //    });
                            //    sql.AppendFormat("SELECT * FROM (SELECT ID,Title,[Index],RefID FROM [ADM].[ADM_VW_ENUM_NAM] WHERE [Enum] = @Enum");
                            //    if (!string.IsNullOrWhiteSpace(data.DSRC_QRY_PARAMS))
                            //    {
                            //        //sql.AppendFormat(" WHERE {0} ", data.DSRC_QRY_PARAMS);
                            //        sql.AppendFormat(" and {0} ", data.DSRC_QRY_PARAMS);
                            //        var matches = Regex.Matches(data.DSRC_QRY_PARAMS, "@[a-zA-Z0-9_]+");
                            //        foreach (Match p in matches)
                            //        {
                            //            var name = p.Value.Substring(1);
                            //            if (parameters.Any(c => c.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                            //                continue;
                            //            var item = data.Params.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            //            if (item != null)
                            //                parameters.Add(new SqlParameter(item.Name, item.Value == null ? DBNull.Value : item.Value));
                            //            else
                            //                parameters.Add(new SqlParameter(name, DBNull.Value));
                            //        }
                            //    }
                            //    sql.Append(" ) T");
                            //    parameters.Add(new SqlParameter("Enum", data.DSRC_SRC));
                            //    // Where Clause
                            //    PrepareWhereClause(data, sql, data, parameters);
                            //    // ORDER BY
                            //    string orderBY = "[Index]";
                            //    if (!string.IsNullOrWhiteSpace(data.Sort))
                            //        orderBY = FixSortExpr(data.Sort);
                            //    else if (!string.IsNullOrWhiteSpace(data.DSRC_ORDER))
                            //    {
                            //        orderBY = $"[{data.DSRC_ORDER}]";
                            //    }
                            //    else if (data.ADM_DYNAMIC_FORM_ITEMs.Any())
                            //    {
                            //        orderBY = $"[{data.ADM_DYNAMIC_FORM_ITEMs.OrderBy(c => c.DFMI_SEQ).First().DFMI_NAM_DISPLAY}]";
                            //    }
                            //    // PAGING
                            //    long totalCount = 0;
                            //    //if (data.Take > 0)                        
                            //    //{
                            //    //    totalCount = UnitOfWork.Context.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM ({0}) X", sql.ToString()), System.Data.CommandType.Text, parameters.Select(p => p.Clone<SqlParameter>()).ToArray());
                            //    //    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                            //    //    sql.AppendFormat("OFFSET ({0}) ROWS ", data.Skip);
                            //    //    sql.AppendFormat("FETCH NEXT {0} ROWS ONLY", data.Take);
                            //    //}
                            //    //else
                            //    //{
                            //    //    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                            //    //}
                            //    //
                            //    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                            //    var sqlData = UnitOfWork.Context.ExecuteReader(sql.ToString(), System.Data.CommandType.Text, parameters.ToArray()).ToObjectList();
                            //    result.TotalRowCount = totalCount > 0 ? totalCount : sqlData.LongCount();
                            //    result.Data = sqlData;
                            //}
                            //
                            //return ;

                            result = _data.Query<dynamic>(re.ToString(), dbparam.ToArray());
                            _data.Dispose();
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _data.Dispose();

                    throw ex;
                }
                finally
                {
                    _data.Dispose();
                }

                return null;
            }

            //var test = JsonConvert.SerializeObject(_data.Query<dynamic>(re.ToString(), dbparam.ToArray()));
            //return JsonConvert.DeserializeObject<dynamic>(test); ;




        }

    }

    //} [WebApi(Route = "api/getDS", Authorized = false, Method = WebApiMethod.Post)]
    //public dynamic Generate(QueryInputParamater input)
    //{

    //    DataSourceInput result = _data.Query<DataSourceInput>("SELECT Top 1 * FROM ADM.ADM_DATA_SOURCES WHERE DSRC_COD=@P1",
    //        new DBParam { Name = "@P1", Value = input.Code }).FirstOrDefault();
    //    string re = "";
    //    List<DBParam> dbparam = new List<DBParam>();
    //    if (result != null)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        if (result.DSRC_TYP_SRC == DataSourceType.Table || result.DSRC_TYP_SRC == DataSourceType.View)
    //        {

    //            sb.Append("SELECT");
    //            if (!string.IsNullOrEmpty(result.DSRC_JSO_FIELDS))
    //            {
    //                dynamic json = JsonConvert.DeserializeObject(result.DSRC_JSO_FIELDS);
    //                for (int i = 0; i < json.Count; i++)
    //                {
    //                    if (i == json.Count - 1)
    //                    {
    //                        sb.AppendFormat(" {0} as {1} ", json[i].name, json[i].alias);
    //                    }
    //                    else
    //                    {
    //                        sb.AppendFormat(" {0} as {1},", json[i].name, json[i].alias);
    //                    }

    //                }

    //            }
    //            sb.AppendFormat("FROM {0} ", result.DSRC_SRC);

    //            if (input.Filters != null && !string.IsNullOrEmpty(result.DSRC_QRY_PARAMS))
    //            {

    //                Match queryparameters = Regex.Match(result.DSRC_QRY_PARAMS.ToString(), @"\@([^=<>\s\']+)");
    //                for (int j = 0; j < input.Filters.Count; j++)
    //                {
    //                    foreach (object param in queryparameters.Captures)
    //                    {
    //                        QueryFilterItems item = input.Filters[j];
    //                        string stringparam = param.ToString();
    //                        //if (stringparam == item.FieldName)
    //                        //{
    //                        //stringparam = param.ToString().Replace("@", "");
    //                        sb.AppendFormat(" WHERE {0} {1} {2} ", item.FieldName, item.Operator == QueryFilterOperator.Equal ? "=" : "", param.ToString());
    //                        //sb.AppendFormat("WHERE ");
    //                        //sb.AppendFormat(result.DSRC_QRY_PARAMS.ToString().Replace(param.ToString(), " '" + item.Value.ToString() + "' "));
    //                        dbparam = new List<DBParam>
    //                        {
    //                            new DBParam { Name = param.ToString(), Value = item.Value }
    //                        };
    //                        //}


    //                    }

    //                }

    //            }
    //            //if (!string.IsNullOrEmpty(result.DSRC_ORDER))
    //            //{
    //            //    sb.AppendFormat(" ORDER BY {0} ", result.DSRC_ORDER);
    //            //}


    //            ////sb.AppendFormat("OFFSET {0} ROWS ", input.Skip.ToString());


    //            //if (input.Take != 0)
    //            //{
    //            //    sb.AppendFormat("FETCH NEXT {0} ROWS ONLY", input.Take.ToString());
    //            //}


    //        }
    //        else if (result.DSRC_TYP_SRC == DataSourceType.Function)
    //        {

    //        }
    //        re = sb.ToString();
    //    }
    //   return JsonConvert.SerializeObject(_data.Query<dynamic>(re));


    //}

}

