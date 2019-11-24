using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Contracts
{
    public interface ISubcontractorsServicesContext
    {
     int Insert(SubcontractorsServicesInputModel data);

        int Delete(SubcontractorsServicesInputModel data);

    }
}