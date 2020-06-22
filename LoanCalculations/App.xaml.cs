using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using CommonServiceLocator;
using LoanHepler.Views;
using Prism.Ioc;
using Prism.Unity;

namespace LoanHepler
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
