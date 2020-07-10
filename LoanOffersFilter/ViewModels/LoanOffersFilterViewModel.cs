using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace LoanOffersFilter.ViewModels
{
    public class LoanOffersFilterViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        private ICollectionView _clientsCollectionView;
        private CollectionViewSource _offersViewSource;

        #endregion

        public LoanOffersFilterViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            OffersViewSource = new CollectionViewSource();
            OffersViewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Offer.Banks)));
            #region Navigation Commands

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadData());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);

            #endregion
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;


        public ICollectionView ClientsCollectionView
        {
            get => _clientsCollectionView;
            set => SetProperty(ref _clientsCollectionView, value);
        }

        public CollectionViewSource OffersViewSource
        {
            get => _offersViewSource;
            set => SetProperty(ref _offersViewSource, value);
        }

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadData()
        {
            await _bankEntities.Clients.LoadAsync();
            await _bankEntities.Banks.LoadAsync();
            await _bankEntities.Offers.LoadAsync();

            ClientsCollectionView = CollectionViewSource.GetDefaultView(_bankEntities.Clients.Local);
            OffersViewSource.Source = _bankEntities.Offers.Local;


            Debug.WriteLine("LoanOffersFilterViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        /// <param name="e">Параметры отмены навигации</param>
        private void NavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatingFrom");
        }

        #endregion
    }
}
