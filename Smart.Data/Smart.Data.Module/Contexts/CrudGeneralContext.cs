using ACoreX.WebAPI;
using ACoreX.WebAPI.Abstractions;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Module.Contexts
{
    public class CRUDGeneralContext : ICRUDGeneral
    {
        private IData _Idata;
        public CRUDGeneralContext(IData data)
        {
            _Idata = data;
        }

        [WebApi(Route = "api/{entityName}/delete", Authorized = false, Method = WebApiMethod.Post)]
        public Task Delete(string entityName, QueryFilterItems model)
        {
            using (var cnn = _Idata.OpenConnection())
            {
                try
                {

                    Entity entity = _Idata.Query<Entity>(
                        "SELECT TOP (1) * FROM [CRM].[ADM].[ADM_ENTITIES] where [ENTT_NAME] = @entityName",
                        new DBParam() { Name = "@entityName", Value = entityName }
                        ).FirstOrDefault();

                    var column = _Idata.Query<EntityFields>(
                     string.Format(
                         "SELECT TOP (1) *   FROM [CRM].[ADM].[ADM_ENTITY_FIELDS] where [ETFD_ALIAS] = @columnName and [ETFD_ENTT_ID] = '{0}'",
                     entity.ENTT_ID.ToString()),
                     new DBParam() { Name = "@columnName", Value = model.FieldName }
                     ).FirstOrDefault();
                    StringBuilder sb = new StringBuilder();

                    //TODO: operator
                    sb.AppendFormat("DELETE FROM {0} WHERE {1} = @param ", entity.ENTT_SOURCE, column.ETFD_NAME);
                    DBParam p1 = new DBParam { Name = "@param", Value = model.Value };
                    var result = _Idata.Execute(sb.ToString(), commandType: System.Data.CommandType.Text, p1);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _Idata.Dispose();
                    throw ex;
                }
            }
        }


        [WebApi(Route = "api/{entityName}/insert", Authorized = false, Method = WebApiMethod.Post)]
        public Task Insert(string entityName, GeneralInsert model)
        {
            using (var cnn = _Idata.OpenConnection())
            {
                try
                {
                    Entity entity = _Idata.Query<Entity>(
                       "SELECT TOP (1) * FROM [CRM].[ADM].[ADM_ENTITIES] where [ENTT_NAME] = @entityName",
                       new DBParam() { Name = "@entityName", Value = entityName }
                       ).FirstOrDefault();
                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    foreach (var item in model.List)
                    {
                        var column = _Idata.Query<EntityFields>(
                        string.Format(
                            "SELECT TOP (1) *   FROM [CRM].[ADM].[ADM_ENTITY_FIELDS] where [ETFD_ALIAS] = @columnName and [ETFD_ENTT_ID] = '{0}'",
                        entity.ENTT_ID.ToString()),
                        new DBParam() { Name = "@columnName", Value = item.FieldName }
                        ).FirstOrDefault();
                        columns.Append(column.ETFD_NAME + ",");
                    }
                    foreach (var item in model.List)
                    {
                        values.Append("'"+item.Value + "'"+ ",");
                    }
                    --columns.Length;
                    --values.Length;
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("Insert Into {0} ({1}) VALUES ({2})", entity.ENTT_SOURCE, columns, values);
                    //DBParam p1 = new DBParam { Name = "@param", Value = model.Values };
                    var result = _Idata.Execute(sb.ToString(), commandType: System.Data.CommandType.Text);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _Idata.Dispose();
                    throw ex;
                }
            }
        }

        //TODO: Update function
        public Task Update(string entityName, GeneralInsert model)
        {
            using (var cnn = _Idata.OpenConnection())
            {
                try
                {
                    Entity entity = _Idata.Query<Entity>(
                       "SELECT TOP (1) * FROM [CRM].[ADM].[ADM_ENTITIES] where [ENTT_NAME] = @entityName",
                       new DBParam() { Name = "@entityName", Value = entityName }
                       ).FirstOrDefault();
                    StringBuilder columns = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    foreach (var item in model.List)
                    {
                        var column = _Idata.Query<EntityFields>(
                        string.Format(
                            "SELECT TOP (1) *   FROM [CRM].[ADM].[ADM_ENTITY_FIELDS] where [ETFD_ALIAS] = @columnName and [ETFD_ENTT_ID] = '{0}'",
                        entity.ENTT_ID.ToString()),
                        new DBParam() { Name = "@columnName", Value = item.FieldName }
                        ).FirstOrDefault();
                        columns.Append(column.ETFD_NAME + ",");
                    }
                    foreach (var item in model.List)
                    {
                        values.Append("'" + item.Value + "'" + ",");
                    }
                    --columns.Length;
                    --values.Length;
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("Insert Into {0} ({1}) VALUES ({2})", entity.ENTT_SOURCE, columns, values);
                    //DBParam p1 = new DBParam { Name = "@param", Value = model.Values };
                    var result = _Idata.Execute(sb.ToString(), commandType: System.Data.CommandType.Text);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _Idata.Dispose();
                    throw ex;
                }
            }
        }
    }
}
