using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AcoreX.Data;
using AcoreX.Data.Repository;
using AcoreX.Helper;
using AcoreX.Utility;
using AcoreX.Utility.Globalization;
using ERP.Core;
using ERP.Infrastructure.Data.Enum.ADM;
using ERP.Infrastructure;
using ERP.Infrastructure.Contexts;
using ERP.Infrastructure.Data;
using ERP.Modules.ADM.Contracts;
using ERP.Modules.ADM.Models;

namespace ERP.Modules.ADM.Contexts
{
    public class DynamicFormContext : BaseContext, IDynamicFormContext
    {
        #region LOV

        [WebApi(Route = "SYS/Forms/List", Method = WebApiMethod.Get, Description = "œ—Ì«›  ·Ì” ", Author = "A.Oshnoudi", CreateDate = "96/7/3", Content = "Forms", CacheDuration = 20)]
        public DynamicFormModel QueryData(DynamicFormQueryFilter data)
        {
            try
            {
                if (data != null)
                {
                    data.SearchQuery = data.SearchQuery?.TrimEnd();
                }
                else
                {
                    throw new InvalidOperationException();
                }
                var result = new DynamicFormModel();
                using (var rep = new Repository<ADM_DYNAMIC_FORM>(UnitOfWork))
                using (var repRef = new Repository<ADM_REFERENCE_CODE>(UnitOfWork))
                {
                    var sql = new StringBuilder();
                    var form = rep.Items.First(c => c.DFRM_COD == data.Code);
                    result.ID = form.DFRM_ID;
                    //
                    result.SaveApi = form.DFRM_UPD_API;
                    //
                    result.Fields = new List<DynamicFormItem>();
                    var parameters = new List<DbParameter>();
                    // View And Tables
                    if (form.DFRM_TYP_SRC == ((int)DataSourceType.Table) || form.DFRM_TYP_SRC == ((int)DataSourceType.View) || form.DFRM_TYP_SRC == (int)DataSourceType.Function)
                    {
                        if (data.Take > 0 && data.Skip != -1 || data.Take == null)
                            sql.Append("SELECT * FROM (SELECT ");
                        else
                            sql.AppendFormat("SELECT TOP {0} * FROM (SELECT ", data.Take);


                        foreach (var item in form.ADM_DYNAMIC_FORM_ITEMs)
                        {
                            result.Fields.Add(new DynamicFormItem
                            {
                                Name = item.DFMI_NAM_DISPLAY,
                                Title = StringManager.GetString(item.DFMI_NAM_CAPTION),
                                Order = item.DFMI_SEQ,
                                Format = item.DFMI_FORMAT,
                                Max = item.DFMI_VAL_MAX,
                                Min = item.DFMI_VAL_MIN,
                                SortOrder = item.DFMI_SEQ_SORT,
                                Visible = item.DFMI_FLG_VISIBLE,
                                ControlType = (FormControlType)item.DFMI_TYP_CTRL,
                                DataType = (FormItemDataType)item.DFMI_TYP_DATA,
                                IsDisplay = item.DFMI_FLG_DISPLAY,
                                IsValue = item.DFMI_FLG_VALUE,
                                Width = item.DFMI_VAL_COL_WIDTH,
                                Required = !item.DFMI_FLG_ALLOW_NULL,
                                Lookup = item.DFMI_NAM_LOV_SRC
                            });
                            sql.AppendFormat("[{0}] AS [{1}],", item.DFMI_NAM, item.DFMI_NAM_DISPLAY);
                        }
                        sql.Length--;
                        if (form.DFRM_TYP_SRC == (int)DataSourceType.Function)
                        {
                            sql.AppendFormat(" FROM {0} ( ", form.DFRM_SRC);
                            if (data.Params.Any())
                            {
                                foreach (var item in data.Params.OrderBy(c => c.Order))
                                {
                                    sql.Append($"@{item.Name},");
                                    parameters.Add(new SqlParameter(item.Name, item.Value == null ? DBNull.Value : item.Value));
                                }
                            }
                            sql.Length--;
                            sql.Append(")");
                        }
                        else
                        {
                            sql.AppendFormat(" FROM {0} ", form.DFRM_SRC);
                            // Query Params 
                            if (!string.IsNullOrWhiteSpace(form.DFRM_QRY_PARAMS))
                            {
                                sql.AppendFormat(" WHERE {0} ", form.DFRM_QRY_PARAMS);
                                var matches = Regex.Matches(form.DFRM_QRY_PARAMS, "@[a-zA-Z0-9_]+");
                                foreach (Match p in matches)
                                {
                                    var name = p.Value.Substring(1);
                                    if (parameters.Any(c => c.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                                        continue;
                                    var item = data.Params.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                                    if (item != null)
                                        parameters.Add(new SqlParameter(item.Name, item.Value == null ? DBNull.Value : item.Value));
                                    else
                                        parameters.Add(new SqlParameter(name, DBNull.Value));
                                }
                            }
                        }
                        sql.Append(") T");
                        // Where Clause
                        PrepareWhereClause(data, sql, form, parameters);
                        // ORDER BY
                        string orderBY = null;
                        if (!string.IsNullOrWhiteSpace(data.Sort))
                            orderBY = FixSortExpr(data.Sort);
                        else if (!string.IsNullOrWhiteSpace(form.DFRM_ORDER))
                        {
                            orderBY = $"[{form.DFRM_ORDER}]";
                        }
                        else if (form.ADM_DYNAMIC_FORM_ITEMs.Any())
                        {
                            orderBY = $"[{form.ADM_DYNAMIC_FORM_ITEMs.OrderBy(c => c.DFMI_SEQ).First().DFMI_NAM_DISPLAY}]";
                        }
                        // PAGING
                        long totalCount = 0;
                        if (data.Take > 0 && data.Skip != -1)
                        {
                            totalCount = UnitOfWork.Context.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM ({0}) X", sql.ToString()), System.Data.CommandType.Text, parameters.Select(p => p.Clone<SqlParameter>()).ToArray());
                            sql.AppendFormat(" ORDER BY {0} ", orderBY);
                            sql.AppendFormat("OFFSET ({0}) ROWS ", data.Skip);
                            sql.AppendFormat("FETCH NEXT {0} ROWS ONLY", data.Take);
                        }
                        else
                        {
                            sql.AppendFormat(" ORDER BY {0} ", orderBY);
                        }
                        //
                        var sqlData = UnitOfWork.Context.ExecuteReader(sql.ToString(), System.Data.CommandType.Text, parameters.ToArray()).ToObjectList();
                        result.TotalRowCount = totalCount > 0 ? totalCount : sqlData.LongCount();
                        result.Data = sqlData;
                    }
                    // Enum Types
                    if (form.DFRM_TYP_SRC == ((int)DataSourceType.Enum))
                    {

                        result.Fields.Add(new DynamicFormItem
                        {
                            Name = "ID",
                            Title = StringManager.GetString("PUB_VALUE"),
                            Order = 0,
                            Visible = true,
                            IsValue = true,
                            ControlType = FormControlType.TextBox,
                            DataType = FormItemDataType.String,
                        });

                        result.Fields.Add(new DynamicFormItem
                        {
                            Name = "Title",
                            Title = StringManager.GetString("PUB_TITLE"),
                            Order = 1,
                            IsDisplay = true,
                            Visible = true,
                            ControlType = FormControlType.TextBox,
                            DataType = FormItemDataType.String,
                        });
                        sql.AppendFormat("SELECT * FROM (SELECT ID,Title,[Index],RefID FROM [ADM].[ADM_VW_ENUM_NAM] WHERE [Enum] = @Enum");
                        if (!string.IsNullOrWhiteSpace(form.DFRM_QRY_PARAMS))
                        {
                            //sql.AppendFormat(" WHERE {0} ", form.DFRM_QRY_PARAMS);
                            sql.AppendFormat(" and {0} ", form.DFRM_QRY_PARAMS);
                            var matches = Regex.Matches(form.DFRM_QRY_PARAMS, "@[a-zA-Z0-9_]+");
                            foreach (Match p in matches)
                            {
                                var name = p.Value.Substring(1);
                                if (parameters.Any(c => c.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                                    continue;
                                var item = data.Params.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                                if (item != null)
                                    parameters.Add(new SqlParameter(item.Name, item.Value == null ? DBNull.Value : item.Value));
                                else
                                    parameters.Add(new SqlParameter(name, DBNull.Value));
                            }
                        }
                        sql.Append(" ) T");
                        parameters.Add(new SqlParameter("Enum", form.DFRM_SRC));
                        // Where Clause
                        PrepareWhereClause(data, sql, form, parameters);
                        // ORDER BY
                        string orderBY = "[Index]";
                        if (!string.IsNullOrWhiteSpace(data.Sort))
                            orderBY = FixSortExpr(data.Sort);
                        else if (!string.IsNullOrWhiteSpace(form.DFRM_ORDER))
                        {
                            orderBY = $"[{form.DFRM_ORDER}]";
                        }
                        else if (form.ADM_DYNAMIC_FORM_ITEMs.Any())
                        {
                            orderBY = $"[{form.ADM_DYNAMIC_FORM_ITEMs.OrderBy(c => c.DFMI_SEQ).First().DFMI_NAM_DISPLAY}]";
                        }
                        // PAGING
                        long totalCount = 0;
                        //if (data.Take > 0)                        
                        //{
                        //    totalCount = UnitOfWork.Context.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM ({0}) X", sql.ToString()), System.Data.CommandType.Text, parameters.Select(p => p.Clone<SqlParameter>()).ToArray());
                        //    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                        //    sql.AppendFormat("OFFSET ({0}) ROWS ", data.Skip);
                        //    sql.AppendFormat("FETCH NEXT {0} ROWS ONLY", data.Take);
                        //}
                        //else
                        //{
                        //    sql.AppendFormat(" ORDER BY {0} ", orderBY);
                        //}
                        //
                        sql.AppendFormat(" ORDER BY {0} ", orderBY);
                        var sqlData = UnitOfWork.Context.ExecuteReader(sql.ToString(), System.Data.CommandType.Text, parameters.ToArray()).ToObjectList();
                        result.TotalRowCount = totalCount > 0 ? totalCount : sqlData.LongCount();
                        result.Data = sqlData;
                    }
                    //
                    return result;
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Context.ClearChanges();
                throw ex;
            }
            finally
            {
                UnitOfWork.Context.CloseConnection();
            }
        }

        private void PrepareWhereClause(DynamicFormQueryFilter data, StringBuilder sql, ADM_DYNAMIC_FORM form, List<DbParameter> parameters)
        {
            sql.AppendLine();
            var index = 0;
            sql.AppendFormat(" WHERE  ");
            if (!String.IsNullOrWhiteSpace(data.SearchQuery))
            {
                if (form.ADM_DYNAMIC_FORM_ITEMs.Any())
                {
                    foreach (var item in form.ADM_DYNAMIC_FORM_ITEMs)
                    {
                        if (item.DFMI_TYP_DATA == (long)FormItemDataType.String)
                        {
                            sql.AppendFormat("[{0}] = @P{1} OR [{0}] LIKE '%'+@P{1}+'%' OR ", item.DFMI_NAM_DISPLAY, index);
                            parameters.Add(new SqlParameter("P" + index, data.SearchQuery));
                            index++;
                        }
                        if (item.DFMI_TYP_DATA == (long)FormItemDataType.Numeric)
                        {
                            sql.AppendFormat("[{0}] = @P{1} OR CONVERT(NVARCHAR(200),[{0}]) LIKE '%'+CONVERT(NVARCHAR(200), @P{1})+'%' OR ", item.DFMI_NAM_DISPLAY, index);
                            long value;
                            parameters.Add(new SqlParameter("P" + index, long.TryParse(data.SearchQuery, out value) ? (object)value : DBNull.Value));
                            index++;
                        }
                        if (item.DFMI_TYP_DATA == (long)FormItemDataType.GUID)
                        {
                            Guid value;
                            sql.AppendFormat("[{0}] = @P{1} OR ", item.DFMI_NAM_DISPLAY, index);
                            parameters.Add(new SqlParameter("P" + index, Guid.TryParse(data.SearchQuery, out value) ? (object)value : DBNull.Value));
                            index++;
                        }
                    }
                }
                else
                {
                    sql.Append("[ID] = @P0 OR [RefID]=@p1 OR [Title] LIKE '%'+@P2+'%' OR ");
                    decimal num;
                    parameters.Add(new SqlParameter("P0", Decimal.TryParse(data.SearchQuery, out num) ? (object)num : DBNull.Value));
                    Guid value;
                    parameters.Add(new SqlParameter("P1", Guid.TryParse(data.SearchQuery, out value) ? (object)value : DBNull.Value));
                    parameters.Add(new SqlParameter("P2", data.SearchQuery));
                }
                sql.AppendFormat("1=2 AND ");
            }
            var param = data.Params.Where(d => !parameters.Any(c => c.ParameterName.Equals(d.Name, StringComparison.OrdinalIgnoreCase)));
            if (param.Any())
            {
                sql.AppendFormat("(");
                foreach (var item in param)
                {
                    sql.AppendFormat("[{0}] LIKE '%'+@P{1}+'%' AND ", item.Name, index);
                    parameters.Add(new SqlParameter("P" + index, item.Value));
                    index++;
                }
                sql.AppendFormat("1=1) AND ");
            }
            sql.AppendFormat("1=1");
        }

        private string FixSortExpr(string expr)
        {
            var parts = expr.Split(',');
            var sorts = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var item in parts)
            {
                var fieldName = item.ToUpper().Trim();
                if (fieldName.EndsWith("DESC"))
                    sorts.Add($"[{fieldName.Remove(fieldName.Length - 4)}] DESC");
                else if (fieldName.EndsWith("ASC"))
                    sorts.Add($"[{fieldName.Remove(fieldName.Length - 3)}] ASC");
                else
                    sorts.Add($"[{fieldName}]");

            }
            return String.Join(",", sorts);
        }

        #endregion

        #region CROD Form

        [WebApi(Route = "SYS/Forms/Save", Method = WebApiMethod.Post, Description = "–ŒÌ—Â ›—„", Author = "A.Oshnoudi", CreateDate = "96/7/12", Content = "Forms")]
        public DynamicFormModel Save(DynamicFormSaveModel data)
        {
            try
            {
                var result = new DynamicFormModel();
                using (var rep = new Repository<ADM_DYNAMIC_FORM>(UnitOfWork))
                {
                    var form = rep.Items.First(c => c.DFRM_COD == data.FormCode);
                    var sql = new StringBuilder();
                    // check data exists
                    var exists = false;
                    sql.AppendFormat("SELECT COUNT(*) FROM {0} WHERE ID = @ID", form.DFRM_SRC);
                    var count = UnitOfWork.Context.ExecuteScalar<int>(sql.ToString(), System.Data.CommandType.Text, new SqlParameter("ID", data.ID));
                    exists = count >= 1;
                    var param = new List<SqlParameter>();
                    if (exists)
                    {
                        sql.Clear();
                        sql.AppendFormat("UPDATE {0} ", form.DFRM_SRC);
                        sql.Append("SET ");
                        foreach (var field in data.Fields)
                        {
                            var column = form.ADM_DYNAMIC_FORM_ITEMs.First(c => c.DFMI_NAM_DISPLAY == field.Name);

                            if (field.Value == null && !column.DFMI_FLG_ALLOW_NULL)
                                throw new DemisException(StringManager.GetString(column.DFMI_NAM_CAPTION));
                            if (column.DFMI_FLG_ALLOW_EDIT)
                            {

                                sql.AppendFormat("{0} = @P{1},", column.DFMI_NAM, param.Count);
                                param.Add(new SqlParameter("P" + param.Count, field.Value));
                            }
                        }
                        sql.Length--;
                        sql.AppendLine(" WHERE ");
                        sql.AppendFormat("{0} = @P{1}", "ID", param.Count);
                        param.Add(new SqlParameter("P" + param.Count, data.ID));
                    }
                    else
                    {
                        sql.Clear();
                        sql.AppendFormat("INSERT INTO {0}", form.DFRM_SRC);
                        sql.AppendLine("(");
                        foreach (var field in data.Fields)
                        {
                            var column = form.ADM_DYNAMIC_FORM_ITEMs.First(c => c.DFMI_NAM_DISPLAY == field.Name);
                            if (column.DFMI_FLG_ALLOW_INSERT)
                                sql.AppendFormat("{0},", column.DFMI_NAM);
                        }
                        sql.Length--;
                        sql.AppendLine(")");
                        sql.AppendLine("VALUES (");
                        foreach (var field in data.Fields)
                        {
                            var column = form.ADM_DYNAMIC_FORM_ITEMs.First(c => c.DFMI_NAM_DISPLAY == field.Name);
                            if (column.DFMI_FLG_ALLOW_INSERT)
                            {
                                sql.AppendFormat("@P{1},", column.DFMI_NAM, param.Count);
                                param.Add(new SqlParameter("P" + param.Count, field.Value));
                            }
                        }
                        sql.Length--;
                        sql.Append(")");
                    }
                    UnitOfWork.Context.ExecuteQuery(sql.ToString(), param.ToArray());
                    return result;
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Context.ClearChanges();
                throw ex;
            }
            finally
            {
                UnitOfWork.Context.CloseConnection();
            }
        }

        [WebApi(Route = "SYS/Forms/Delete", Method = WebApiMethod.Post, Description = "Õ–› —òÊ—œ", Author = "A.Oshnoudi", CreateDate = "96/7/11", Content = "Forms")]
        public void Delete(DynamicFormDeleteModel data)
        {
            try
            {
                using (var rep = new Repository<ADM_DYNAMIC_FORM>(UnitOfWork))
                {
                    var form = rep.Items.First(c => c.DFRM_COD == data.FormCode);
                    var sql = new StringBuilder();
                    var param = new List<SqlParameter>();
                    foreach (var id in data.List)
                    {
                        sql.AppendFormat("DELETE FROM {0} WHERE ID = @P{1};", form.DFRM_SRC, param.Count);
                        param.Add(new SqlParameter("P" + param.Count, id));
                        sql.AppendLine();
                    }

                    UnitOfWork.Context.ExecuteQuery(sql.ToString(), param.ToArray());
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Context.ClearChanges();
                throw ex;
            }
            finally
            {
                UnitOfWork.Context.CloseConnection();
            }
        }


        #endregion
    }
}
