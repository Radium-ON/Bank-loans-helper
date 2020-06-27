using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using CommonServiceLocator;
using LoanHelper.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace LoanHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Overrides of PrismApplicationBase

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IBankEntitiesContext), typeof(BankLoansEntities));
        }

        protected override Window CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ClientInfo.ClientInfoModule>();
            moduleCatalog.AddModule<Core.CoreModule>();
        }

        #endregion
    }
}
