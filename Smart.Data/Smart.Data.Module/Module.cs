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

            builder.AddTransient<IQueryGenerator, QueryGeneratorContext>();
            builder.AddTransient<IEntitiesNoteContext, EntitiesNoteContext>();
            builder.AddTransient<ICRUDGeneral, CRUDGeneralContext>();
        }
    }
}
