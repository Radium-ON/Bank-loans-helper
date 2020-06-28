using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanHelper.Core.ViewModels;
using LoanHelper.Core.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace LoanHelper.Core
{
    public class CoreModule : IModule
    {
        #region Implementation of IModule

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<OkCancelDialog,OkCancelDialogViewModel>("OkCancelDialog");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        #endregion
    }
}
