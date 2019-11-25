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


            return 0;
        }


        [WebApi(Route = "api/CRM/PartnersContacts/Delete", Authorized = false, Method = WebApiMethod.Post)]

        public int Insert(PartnersContactInputModel data)
        {


            return 0;

        }
    }
}
