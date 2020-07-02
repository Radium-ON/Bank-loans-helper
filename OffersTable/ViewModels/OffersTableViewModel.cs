using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OffersTableViewModel : ModernViewModelBase
    {
        private readonly IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public OffersTableViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            OfferViewModels = new ObservableCollection<OfferInfoViewModel>();

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadData());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        #region Backing Fields
        private ObservableCollection<OfferInfoViewModel> _offerViewModels;



        #endregion

        public ObservableCollection<OfferInfoViewModel> OfferViewModels
        {
            get => _offerViewModels;
            set => SetProperty(ref _offerViewModels, value);
        }

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("OffersTableViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadData()
        {
            await _bankEntities.Offers.LoadAsync();
            OfferViewModels.AddRange(GetOfferInfoViewModels());
            Debug.WriteLine("OffersTableViewModel - LoadData");
        }

        private List<OfferInfoViewModel> GetOfferInfoViewModels()
        {
            return _bankEntities.Offers.Local.Select(offer => new OfferInfoViewModel(offer,_dialogService,_bankEntities,_eventAggregator)).ToList();
        }


        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("OffersTableViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("OffersTableViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("OffersTableViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        /// <param name="e">Параметры отмены навигации</param>
        private void NavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("OffersTableViewModel - NavigatingFrom");
        }
    }
}
