using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Diploma.BLL.Interfaces.Services;
using Diploma.BLL.Services;
using Diploma.DAL.Contexts;
using Diploma.Framework.Interfaces;
using Diploma.Framework.Services;
using Diploma.Validators;
using Diploma.ViewModels;
using FluentValidation;
using SimpleInjector;

namespace Diploma
{
    public sealed class AppBootstrapper : BootstrapperBase
    {
        private static readonly Container Container = new Container();

        public AppBootstrapper()
        {
            Initialize();
            ChangeLocalization();
        }

        protected override void BuildUp(object instance)
        {
            var registration = Container.GetRegistration(instance.GetType(), true);
            registration.Registration.InitializeInstance(instance);
        }

        protected override void Configure()
        {
            Container.RegisterSingleton<Func<CompanyContext>>(() => Container.GetInstance<CompanyContext>());

            Container.RegisterSingleton<IWindowManager, WindowManager>();
            Container.RegisterSingleton<IEventAggregator, EventAggregator>();
            Container.RegisterSingleton<IMessageService, MessageService>();
            Container.RegisterSingleton<IUserService, UserService>();
            Container.RegisterSingleton<ICryptoService, CryptoService>();

            Container.Register<ShellViewModel>();
            
            Container.Register<AuthenticationManagerViewModel>();

            Container.Register<RegisterViewModel>();
            Container.RegisterSingleton<Func<RegisterViewModel>>(() => Container.GetInstance<RegisterViewModel>());

            Container.Register<LoginViewModel>();
            Container.RegisterSingleton<Func<LoginViewModel>>(() => Container.GetInstance<LoginViewModel>());
            
            Container.Register<DashboardViewModel>();

            Container.Register<EditUserDataViewModel>();

            Container.RegisterSingleton<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
            Container.RegisterSingleton<IValidator<LoginViewModel>, LoginViewModelValidator>();
            Container.RegisterSingleton<IValidator<EditUserDataViewModel>, EditUserDataViewModelValidator>();

            Container.Verify();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            IServiceProvider provider = Container;
            var collectionType = typeof(IEnumerable<>).MakeGenericType(service);
            var services = (IEnumerable<object>)provider.GetService(collectionType);
            return services ?? Enumerable.Empty<object>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return Container.GetInstance(service);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Container.Dispose();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            yield return Assembly.GetExecutingAssembly();
        }

        private void ChangeLocalization()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
    }
}
