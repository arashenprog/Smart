using ACoreX.Injector.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Smart.Run
{
    public class NetCoreContainerBuilder : IContainerBuilder
    {
        public IServiceCollection Services
        {
            get;
            private set;
        }



        public NetCoreContainerBuilder(IServiceCollection Services)
        {
            this.Services = Services;
        }

        public void Register<T>()
            where T : class, IModule
        {
            T module = (T)Activator.CreateInstance(typeof(T));
            module.Register(this);
        }

        public T Create<T>()
        {
            ServiceProvider sp = Services.BuildServiceProvider();
            return sp.GetService<T>();
        }

       

        void IContainerBuilder.AddSingleton<TService, TImplementation>()
        {
            Services.AddSingleton(typeof(TService), typeof(TImplementation));
        }

        void IContainerBuilder.AddSingleton<TService, TImplementation>(TImplementation instance)
        {
            Services.AddSingleton(typeof(TService), instance);
        }

      

        void IContainerBuilder.AddScope<TService, TImplementation>(TImplementation instance)
        {
            Services.AddScoped(typeof(TService), c => instance);
        }
        void IContainerBuilder.AddScope<TService, TImplementation>()
        {
            Services.AddScoped(typeof(TService), typeof(TImplementation));
        }

        void IContainerBuilder.AddTransient<TService, TImplementation>()
        {
            Services.AddTransient(typeof(TService), typeof(TImplementation));
        }
        void IContainerBuilder.AddTransient<TService, TImplementation>(TImplementation instance)
        {
            Services.AddTransient(typeof(TService), c=> instance);
        }
    }
}
