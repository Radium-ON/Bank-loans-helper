using System.Diagnostics;
using BankLoansDataModel.Services;
using Prism.Commands;

namespace LoanHelper.ViewModels
{
    public class AllClientsViewModel : ModernViewModelBase
    {
        private readonly IBankEntitiesContext _bankEntities;

        public AllClientsViewModel(IBankEntitiesContext bankEntities)
        {
            _bankEntities = bankEntities;

            NavigatingFromCommand = new DelegateCommand(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(LoadData);
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("AllClientsViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private void LoadData()
        {
            Debug.WriteLine("AllClientsViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("AllClientsViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("AllClientsViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("AllClientsViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        private void NavigatingFrom()
        {
            Debug.WriteLine("AllClientsViewModel - NavigatingFrom");
        }
    }
}
