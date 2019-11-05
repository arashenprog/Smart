using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Smart.Data.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Module.Contexts
{
    public class CrudGeneralContext : ICRUDGeneral
    {
        private IData _Idata;
        public CrudGeneralContext(IData data)
        {
            _Idata = data;
        }

        [WebApi(Route = "api/{entityName}/delete", Authorized = false, Method = WebApiMethod.Post)]
        public Task Delete(string entityName, GeneralDataModel model)
        {
            using (SqlConnection cnn = _Idata.OpenConnection())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("DELETE FROM {0} WHERE {1} = @param ", entityName, model.Conditions);
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
