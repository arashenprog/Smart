using ACoreX.Data.Abstractions;
using ACoreX.Data.Dapper;
using ACoreX.Injector.Abstractions;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Module.Contexts;
using System;

namespace Smart.Data.Module
{
    public class Module : IModule
    {
        public void Register(IContainerBuilder builder)
        {
            builder.AddScope<IData, DapperData>(new DapperData("Server=192.168.25.111;Database=CRM; Integrated Security = true;"));
            builder.AddScope<IQueryGenerator, QueryGeneratorContext>();
        }
    }
}
