using Smart.APIs.Module.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.APIs.Module.Contracts
{
    public interface IPartnersContactsContext
    {
        int Insert(PartnersContactInputModel data);
        int Delete(PartnersContactInputModel data);
    }
}
