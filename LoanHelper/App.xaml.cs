﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using ClientInfo;
using CommonServiceLocator;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using LoanHelper.Core;
using LoanHelper.Core.Interfaces;
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
        private LinkCollection _linkCollection = default;
        private const string MODULES_PATH = @".\Modules";
        public App()
        {
            //Инициализация QuickConverter
            EquationTokenizer.AddNamespace(typeof(object));
            EquationTokenizer.AddNamespace(typeof(Visibility));
        }
        #region Overrides of PrismApplicationBase

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IBankEntitiesContext), typeof(BankEntitiesContext));
            containerRegistry.RegisterDialogWindow<PrismModernDialog>();
        }

        protected override Window CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
            var modernWindow = shell as ModernWindow;
            var linkGroupCollection = modernWindow?.MenuLinkGroups.Single(l => l.DisplayName == "Данные");
            linkGroupCollection?.Links.AddRange(_linkCollection);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog { ModulePath = MODULES_PATH };
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var directoryCatalog = (DirectoryModuleCatalog)moduleCatalog;
            directoryCatalog.Initialize();

            _linkCollection = new LinkCollection();
            var typeFilter = new TypeFilter(InterfaceFilter);

            foreach (var module in directoryCatalog.Items)
            {
                var mi = (ModuleInfo)module;
                var asm = Assembly.LoadFrom(mi.Ref);

                foreach (Type t in asm.GetTypes())
                {
                    var myInterfaces = t.FindInterfaces(typeFilter, typeof(IModernLinkService).ToString());

                    if (myInterfaces.Length > 0)
                    {
                        var linkService = (IModernLinkService)asm.CreateInstance(t.FullName);
                        var modernLink = linkService?.GetModernLink();
                        _linkCollection.Add(modernLink);
                    }
                }
            }
            moduleCatalog.AddModule<CoreModule>();
        }

        #endregion

        private Uri GetUri(Type viewType)
        {
            return new Uri($"/Views/{viewType.Name}.xaml", UriKind.Relative);
        }

        private bool InterfaceFilter(Type typeObj, object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }
    }
}