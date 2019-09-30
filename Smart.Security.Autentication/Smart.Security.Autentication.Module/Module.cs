using ACoreX.Injector.Abstractions;
using Smart.Security.Autentication.Abstractions.Contracts;
using Smart.Security.Autentication.Contexts;
using System;

namespace Smart.Security.Autentication
{
    public class Module : IModule
    {
        public void Register(IContainerBuilder builder)
        {
            builder.AddTransient<IAutenticationContext, AutenticationContext>();
        }
    }
}
