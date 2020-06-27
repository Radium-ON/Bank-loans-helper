using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using ClientInfo;
using CommonServiceLocator;
using LoanHelper.Core;
using LoanHelper.Views;
using LoanHelper.Views.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using QuickConverter;

namespace LoanHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            //Инициализация QuickConverter
            EquationTokenizer.AddNamespace(typeof(object));
            EquationTokenizer.AddNamespace(typeof(Visibility));
        }
        #region Overrides of PrismApplicationBase

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IBankEntitiesContext), typeof(BankLoansEntities));
            containerRegistry.RegisterDialogWindow<PrismModernDialog>();
        }

        protected override Window CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ClientInfoModule>();
            moduleCatalog.AddModule<CoreModule>();
        }

        #endregion
    }
}
