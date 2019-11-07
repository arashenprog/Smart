using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
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
            using (SqlConnection cnn = _Idata.OpenConnection())
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

                    sb.AppendFormat("DELETE FROM {0} WHERE {1} {2} @param ", entity.ENTT_SOURCE, column.ETFD_NAME);
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
        public Task Insert(string entityName, GeneralDataModel model)
        {
            using (SqlConnection cnn = _Idata.OpenConnection())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("Insert Into {0} ({1}) VALUES @param", entityName, model.Columns);
                    DBParam p1 = new DBParam { Name = "@param", Value = model.Values };
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


        public Task Update(string entityName, GeneralDataModel model)
        {
            throw new NotImplementedException();
        }
    }
}
