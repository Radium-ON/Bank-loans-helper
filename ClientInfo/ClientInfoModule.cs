﻿using ClientInfo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ClientInfo
{
    public class ClientInfoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("NewClientEditingForm", typeof(ClientView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}