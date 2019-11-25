using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using System.Data.SqlClient;

namespace Smart.Data.Module.Contexts
{
    public class SubcontractorsServiceContext :ISubcontractorsServicesContext
    {
        private IData _data;
        public SubcontractorsServiceContext(IData input)
        {
            _data = input;
        }

     
        [WebApi(Route = "api/CRM/SubcontractorsService/Insert", Authorized = false, Method = WebApiMethod.Post)]

        public int Insert(SubcontractorsServicesInputModel data)
        {
            using (var cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@servicesId";
                    p0.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.ServicesId);


                    DBParam p1 = new DBParam();
                    p1.Name = "@subcontractorsId";
                    p1.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.SubcontractorsId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_SERVICES_INSERT]", System.Data.CommandType.StoredProcedure, p0, p1);


                    _data.Dispose();
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    _data.Dispose();
                }
            }

        }


        [WebApi(Route = "api/CRM/SubcontractorsService/Delete", Authorized = false, Method = WebApiMethod.Post)]

        public int Delete (SubcontractorsServicesInputModel data)
        {

            using (var cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@servicesId";
                    p0.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.ServicesId);


                    DBParam p1 = new DBParam();
                    p1.Name = "@subcontractorsId";
                    p1.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.SubcontractorsId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_SERVICES_DELETE]", System.Data.CommandType.StoredProcedure, p0, p1);


                    _data.Dispose();
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    _data.Dispose();
                }
            }
        }

    }
}
