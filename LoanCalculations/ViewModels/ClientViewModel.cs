using System.Diagnostics;
using Prism.Commands;

namespace LoanHelper.ViewModels
{
    public class ClientViewModel : ModernViewModelBase
    {
        public ClientViewModel()
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
            Debug.WriteLine("ClientViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            Debug.WriteLine("ClientViewModel - LoadData");
        }

        /// <summary>
        /// Navigateds from.
        /// </summary>
        private void NavigatedFrom()
        {
            // called when we navigated to another view
            Debug.WriteLine("ClientViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Navigateds to.
        /// </summary>
        private void NavigatedTo()
        {
            // called when we navigate to the view related with this view model.
            Debug.WriteLine("ClientViewModel - NavigatedTo");
        }

        /// <summary>
        /// Fragments the navigation.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("ClientViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Navigatings from.
        /// </summary>
        private void NavigatingFrom()
        {
            // Called when we will navigate to new view
            Debug.WriteLine("ClientViewModel - NavigatingFrom");
        }
    }
}
