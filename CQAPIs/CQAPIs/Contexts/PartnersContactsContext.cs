using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Smart.APIs.Module.Contracts;
using Smart.APIs.Module.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.APIs.Module.Contexts
{
    public class PartnersContactsContext :IPartnersContactsContext
    {
        private IData _data;
        public PartnersContactsContext(IData input)
        {
            _data = input;
        }

        [WebApi(Route = "api/CRM/PartnersContacts/Insert", Authorized = false, Method = WebApiMethod.Post)]

        public int Delete(PartnersContactInputModel data)
        {
            using (var cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@partnersId";
                    p0.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.PartnersId);


                    DBParam p1 = new DBParam();
                    p1.Name = "@contactsId";
                    p1.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.ContactsId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_CONTACTS_INSERT]", System.Data.CommandType.StoredProcedure, p0, p1);


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


        [WebApi(Route = "api/CRM/PartnersContacts/Delete", Authorized = false, Method = WebApiMethod.Post)]

        public int Insert(PartnersContactInputModel data)
        {
            using (var cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@partnersId";
                    p0.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.PartnersId);


                    DBParam p1 = new DBParam();
                    p1.Name = "@contactsId";
                    p1.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.ContactsId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_CONTACTS_DELETE]", System.Data.CommandType.StoredProcedure, p0, p1);


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
