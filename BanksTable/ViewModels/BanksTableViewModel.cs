using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Extensions;
using BankLoansDataModel.Services;
using BanksTable.Views;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core.Extensions;
using LoanHelper.Core.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace BanksTable.ViewModels
{
    public class BanksTableViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private AsyncObservableCollection<BankInfoViewModel> _bankInfoViewModels;

        private readonly IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        #endregion

        public BanksTableViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            DeleteBankCommand = new DelegateCommand<BankInfoViewModel>(async vm => await DeleteSelectedBankAsync(vm));
            AddBankCommand = new DelegateCommand(ShowAddBankDialog);

            BankInfoViewModels = new AsyncObservableCollection<BankInfoViewModel>();

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadData());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        private void ShowAddBankDialog()
        {
            _dialogService.ShowDialog(nameof(BankAddingDialog), new DialogParameters { { "BankInfoViewModel", new BankInfoViewModel(new Bank(), _bankEntities) } },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedBankVm = r.Parameters.GetValue<BankInfoViewModel>("AddedBankViewModel");

                        _bankEntities.Banks.Add(addedBankVm.Bank);
                        await _bankEntities.SaveChangesAsync(CancellationToken.None);

                        BankInfoViewModels.Add(addedBankVm);
                    }
                });
        }



        public AsyncObservableCollection<BankInfoViewModel> BankInfoViewModels
        {
            get => _bankInfoViewModels;
            set => SetProperty(ref _bankInfoViewModels, value);
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        #region DelegateCommands

        public DelegateCommand<BankInfoViewModel> DeleteBankCommand { get; private set; }
        public DelegateCommand AddBankCommand { get; private set; }

        #endregion

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("BanksTableViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadData()
        {
            BankInfoViewModels.Clear();
            BankInfoViewModels.AddRange(await GetBankInfoViewModelsAsync(_bankEntities.Banks));
            Debug.WriteLine("BanksTableViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("BanksTableViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("BanksTableViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("BanksTableViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        private void NavigatingFrom(NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var dbcontext = _bankEntities as DbContext;
            var hasChanges = dbcontext?.ChangeTracker.HasChanges();

            if (hasChanges == true)
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("some_data_changed_dialog_title") as string,
                    Application.Current.FindResource("some_data_changed_dialog_message") as string,
                    async r => { await NavigatingWithModifiedBanksCallBack(r, navigatingCancelEventArgs, dbcontext); });
            }
            Debug.WriteLine("BanksTableViewModel - NavigatingFrom");
        }

        #endregion

        private async Task DeleteSelectedBankAsync(BankInfoViewModel bankVm)
        {
            if (bankVm == null) return;

            if (bankVm.Bank.LoanAgreements.Count == 0 && bankVm.Bank.Offers.Count == 0)
            {
                await RemoveOfferAsync(bankVm);
            }
            else
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("bank_deleted_dialog_title") as string,
                    Application.Current.FindResource("bank_deleted_dialog_message") as string,
                    async r =>
                    {
                        if (r.Result == ButtonResult.OK)
                        {
                            await RemoveOfferAsync(bankVm);
                        }
                    });
            }
        }

        private async Task RemoveOfferAsync(BankInfoViewModel bankVm)
        {
            _bankEntities.Banks.Remove(bankVm.Bank);
            BankInfoViewModels.Remove(bankVm);
            await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<BankInfoViewModel>> GetBankInfoViewModelsAsync(IDbSet<Bank> banks)
        {
            await banks.LoadAsync();
            return banks.Local.Select(bank => new BankInfoViewModel(bank, _bankEntities));
        }

        private async Task NavigatingWithModifiedBanksCallBack(IDialogResult r, NavigatingCancelEventArgs e, DbContext dbcontext)
        {
            if (r.Result == ButtonResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (r.Result == ButtonResult.OK)
            {
                var updatedBanks = CurrentObjectContext.GetEntriesByEntityState<Bank>(EntityState.Modified);
                var badBanks = await GetNotValidBankViewModelsAsync(BankInfoViewModels);
                if (badBanks.Count == 0)
                {
                    var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                    if (!status.IsValid)
                    {
                        e.Cancel = true;

                        _dialogService.ShowOkDialog("Ошибка обновления", string.Join("\n", status.EfErrors), m => { });
                        await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                    }
                }
                else
                {
                    e.Cancel = true;
                    await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                }
            }
        }

        /// <summary>
        /// Возвращает список оболочек неисправных клиентов <see cref="BankInfoViewModel"/>; асинхронный.
        /// </summary>
        /// <param name="clientInfoViewModels"></param>
        /// <returns>Список неуникальных клиентов.</returns>
        private async Task<List<BankInfoViewModel>> GetNotValidBankViewModelsAsync(IEnumerable<BankInfoViewModel> clientInfoViewModels)
        {
            return await clientInfoViewModels.AsAsyncQueryable().Where(vm => vm.IsValid == false).ToListAsync();
        }
    }
}
