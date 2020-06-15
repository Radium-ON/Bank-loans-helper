﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;

namespace LoanCalculations.ViewModels
{
    public class AllOffersViewModel : ModernViewModelBase
    {
        public AllOffersViewModel()
        {
            NavigatingFromCommand = new DelegateCommand(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(LoadData);
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        /// <summary>
        /// Visibilities the changed.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("AllOffersViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            Debug.WriteLine("AllOffersViewModel - LoadData");
        }

        /// <summary>
        /// Navigateds from.
        /// </summary>
        private void NavigatedFrom()
        {
            // called when we navigated to another view
            Debug.WriteLine("AllOffersViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Navigateds to.
        /// </summary>
        private void NavigatedTo()
        {
            // called when we navigate to the view related with this view model.
            Debug.WriteLine("AllOffersViewModel - NavigatedTo");
        }

        /// <summary>
        /// Fragments the navigation.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("AllOffersViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Navigatings from.
        /// </summary>
        private void NavigatingFrom()
        {
            // Called when we will navigate to new view
            Debug.WriteLine("AllOffersViewModel - NavigatingFrom");
        }
    }
}
