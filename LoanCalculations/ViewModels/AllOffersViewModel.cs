using System.Diagnostics;
using BankLoansDataModel.Services;
using Prism.Commands;

namespace LoanHelper.ViewModels
{
    public class AllOffersViewModel : ModernViewModelBase
    {
        private readonly IBankEntitiesContext _bankEntities;

        public AllOffersViewModel(IBankEntitiesContext bankEntities)
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
            Debug.WriteLine("AllOffersViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private void LoadData()
        {
            Debug.WriteLine("AllOffersViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("AllOffersViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("AllOffersViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("AllOffersViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        private void NavigatingFrom()
        {
            Debug.WriteLine("AllOffersViewModel - NavigatingFrom");
        }
    }
}
