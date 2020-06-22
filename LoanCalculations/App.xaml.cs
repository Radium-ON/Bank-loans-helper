using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using CommonServiceLocator;
using LoanHelper.Views;
using Prism.Ioc;

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

        #endregion
    }
}
