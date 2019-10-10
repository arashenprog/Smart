using ACoreX.Injector.Abstractions;
using Smart.Security.Autentication.Contexts;
using Smart.Utility.Importer.Contracts.Contracts;
using Smart.Utility.Importer.Module.Contexts;
using System;

namespace Smart.Utility.Importer.Module
{
    public class Module : IModule
    {
        public void Register(IContainerBuilder builder)
        {
            builder.AddTransient<IImporterContext, ImporterContexts>();
        }
    }
}
