using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Autofac;
using Autofac.Features.ResolveAnything;
using Caliburn.Micro;
using F1Telemetry;
using F1TelemetryUi.Referencing;
using F1TelemetryUi.ViewModels;

namespace F1TelemetryUi
{
    public class AppBootstrapper : BootstrapperBase
    {
        private Autofac.IContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected Autofac.IContainer Container => _container;

        protected override void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray()).
                Where(x => x.Name.EndsWith("ViewModel", StringComparison.Ordinal)).
                Where(x => !(string.IsNullOrWhiteSpace(x.Namespace))
                    && x.Namespace.EndsWith("ViewModels", StringComparison.Ordinal)).
                Where(x => x.GetInterface(typeof(INotifyPropertyChanged).Name) != null).
                AsSelf().
                InstancePerDependency();

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray()).
                Where(x => x.Name.EndsWith("View", StringComparison.Ordinal)).
                Where(x => !(string.IsNullOrWhiteSpace(x.Namespace))
                    && x.Namespace.EndsWith("Views", StringComparison.Ordinal)).
                AsSelf().
                InstancePerDependency();

            var telemetryManager = new TelemetryManager();
            builder.Register(c => telemetryManager).AsSelf().SingleInstance();
            builder.Register(c => new F1Manager(telemetryManager)).AsSelf().SingleInstance();
            builder.Register(c => new ReferencingStateMachine()).AsSelf().SingleInstance();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            _container = builder.Build();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override object GetInstance(Type service, string key)
        {
            if (!string.IsNullOrWhiteSpace(key) 
                && Container.TryResolveKeyed(key, service, out object instance))
            {
                return instance;
            }

            if (Container.IsRegistered(service))
            {
                return Container.Resolve(service);
            }

            throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
