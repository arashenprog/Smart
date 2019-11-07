using ACoreX.Data.Abstractions;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Abstractions.Contracts
{
    public interface ICRUDGeneral
    {
        Task Insert(string entityName, GeneralDataModel model);
        Task Update(string entityName, GeneralDataModel model);
        Task Delete(string entityName, QueryFilterItems model);

    }
}
