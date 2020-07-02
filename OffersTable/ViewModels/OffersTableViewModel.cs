using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core.Extensions;
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
            DeleteOfferCommand = new DelegateCommand<OfferInfoViewModel>(async vm => await DeleteSelectedOffer(vm));

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

        #region DelegateCommands

        public DelegateCommand<OfferInfoViewModel> DeleteOfferCommand { get; private set; }

        #endregion

        #region NavigationEvents Methods

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
            OfferViewModels.Clear();
            OfferViewModels.AddRange(await GetOfferInfoViewModelsAsync(_bankEntities.Offers));
            Debug.WriteLine("OffersTableViewModel - LoadData");
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

        #endregion

        private async Task DeleteSelectedOffer(OfferInfoViewModel offerVm)
        {
            if (offerVm == null) return;

            if (offerVm.GetOffer().Banks.Count == 0)
            {
                await RemoveOfferAsync(offerVm);
            }
            else
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("offer_deleted_dialog_title") as string,
                    Application.Current.FindResource("offer_deleted_dialog_message") as string,
                    async r =>
                    {
                        if (r.Result == ButtonResult.OK)
                        {
                            await RemoveOfferAsync(offerVm);
                        }
                    });
            }
        }

        private async Task RemoveOfferAsync(OfferInfoViewModel offerVm)
        {
            _bankEntities.Offers.Remove(offerVm.GetOffer());
            OfferViewModels.Remove(offerVm);
            await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<OfferInfoViewModel>> GetOfferInfoViewModelsAsync(IDbSet<Offer> offers)
        {
            await offers.LoadAsync();
            return offers.Local.Select(offer => new OfferInfoViewModel(offer));
        }
    }
}
