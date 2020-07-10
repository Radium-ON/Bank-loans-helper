using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Presentation;
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
        private ObservableCollection<Offer> _offers;

        private int? _monthsInput;
        private decimal? _loanAmountInput;
        private float? _interestInput;
        private bool _canRemoveMonthsFilter;
        private bool _canRemoveLoanAmountFilter;
        private bool _canRemoveInterestFilter;

        #endregion

        public LoanOffersFilterViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            OffersViewSource = new CollectionViewSource();
            OffersViewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Offer.Banks)));

            InitializeCommands();

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

        public ObservableCollection<Offer> Offers
        {
            get => _offers;
            set => SetProperty(ref _offers, value);
        }

        #region Filter Properties

        public int? MonthsInput
        {
            get => _monthsInput;
            set => SetProperty(ref _monthsInput, value, () => ApplyFilter(_monthsInput.HasValue ? FilterField.Months : FilterField.None));
        }

        public decimal? LoanAmountInput
        {
            get => _loanAmountInput;
            set => SetProperty(ref _loanAmountInput, value, () => ApplyFilter(_loanAmountInput.HasValue ? FilterField.LoanAmount : FilterField.None));
        }

        public float? InterestInput
        {
            get => _interestInput;
            set => SetProperty(ref _interestInput, value, () => ApplyFilter(_interestInput.HasValue ? FilterField.Interest : FilterField.None));
        }



        /// <summary>
        /// Gets or sets a flag indicating if the Months filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveMonthsFilter
        {
            get => _canRemoveMonthsFilter;
            set => SetProperty(ref _canRemoveMonthsFilter, value);
        }

        /// <summary>
        /// Gets or sets a flag indicating if the LoanAmount filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveLoanAmountFilter
        {
            get => _canRemoveLoanAmountFilter;
            set => SetProperty(ref _canRemoveLoanAmountFilter, value);
        }

        /// <summary>
        /// Gets or sets a flag indicating if the Interest filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveInterestFilter
        {
            get => _canRemoveInterestFilter;
            set => SetProperty(ref _canRemoveInterestFilter, value);
        }

        #endregion

        #region Commands

        public ICommand ResetFiltersCommand
        {
            get;
            private set;
        }
        public ICommand RemoveMonthsFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveLoanAmountFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveInterestFilterCommand
        {
            get;
            private set;
        }

        #endregion

        private void InitializeCommands()
        {
            ResetFiltersCommand = new RelayCommand(ResetFilters, null);
            RemoveMonthsFilterCommand = new RelayCommand(RemoveMonthsFilter, o => CanRemoveMonthsFilter);
            RemoveLoanAmountFilterCommand = new RelayCommand(RemoveLoanAmountFilter, o => CanRemoveLoanAmountFilter);
            RemoveInterestFilterCommand = new RelayCommand(RemoveInterestFilter, o => CanRemoveInterestFilter);
        }

        /// <summary>
        /// Очищает фильтры отпиской от событий и обнулением свойств выбора
        /// </summary>
        /// <param name="o"></param>
        public void ResetFilters(object o)
        {
            RemoveInterestFilter(o);
            RemoveLoanAmountFilter(o);
            RemoveMonthsFilter(o);
        }

        public void RemoveMonthsFilter(object o)
        {
            OffersViewSource.Filter -= FilterByMonths;
            LoanAmountInput = null;
            CanRemoveMonthsFilter = false;
        }

        public void RemoveLoanAmountFilter(object o)
        {
            OffersViewSource.Filter -= FilterByLoanAmount;
            MonthsInput = null;
            CanRemoveLoanAmountFilter = false;
        }

        public void RemoveInterestFilter(object o)
        {
            OffersViewSource.Filter -= FilterByInterest;
            InterestInput = null;
            CanRemoveInterestFilter = false;
        }

        /* Notes on Adding Filters:
         *   Each filter is added by subscribing a filter method to the Filter event
         *   of the OffersViewSource.  Filters are applied in the order in which they were added. 
         *   To prevent adding filters mulitple times ( because we are using drop down lists
         *   in the view), the CanRemove***Filter flags are used to ensure each filter
         *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
         *   is set to true.       
         *   
         *   If a filter has been applied already (for example someone selects "Canada" to filter by country
         *   and then they change their selection to another value (say "Mexico") we need to undo the previous
         *   country filter then apply the new one.  This does not completey Reset the filter, just
         *   allows it to be changed to another filter value. This applies to the other filters as well
         */

        public void AddMonthsFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveMonthsFilter)
            {
                OffersViewSource.Filter -= FilterByMonths;
                OffersViewSource.Filter += FilterByMonths;
            }
            else
            {
                OffersViewSource.Filter += FilterByMonths;
                CanRemoveMonthsFilter = true;
            }
        }

        public void AddLoanAmountFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveLoanAmountFilter)
            {
                OffersViewSource.Filter -= FilterByLoanAmount;
                OffersViewSource.Filter += FilterByLoanAmount;
            }
            else
            {
                OffersViewSource.Filter += FilterByLoanAmount;
                CanRemoveLoanAmountFilter = true;
            }
        }

        public void AddInterestFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveInterestFilter)
            {
                OffersViewSource.Filter -= FilterByInterest;
                OffersViewSource.Filter += FilterByInterest;
            }
            else
            {
                OffersViewSource.Filter += FilterByInterest;
                CanRemoveInterestFilter = true;
            }
        }

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
         * only hide things which do not match the filter criteria
         * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
         * clears out any previous filters applied to it.  
         */

        private void FilterByLoanAmount(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (LoanAmountInput > src.MaxLoanAmount || LoanAmountInput < src.MinLoanAmount)
                e.Accepted = false;
        }
        private void FilterByInterest(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (InterestInput < src.Interest)
                e.Accepted = false;
        }
        private void FilterByMonths(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (MonthsInput > src.MaxOfMonths)
                e.Accepted = false;
        }

        private enum FilterField
        {
            LoanAmount,
            Months,
            Interest,
            None
        }
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.LoanAmount:
                    AddLoanAmountFilter();
                    break;
                case FilterField.Months:
                    AddMonthsFilter();
                    break;
                case FilterField.Interest:
                    AddInterestFilter();
                    break;
                default:
                    break;
            }
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

            Offers = _bankEntities.Offers.Local;

            ClientsCollectionView = CollectionViewSource.GetDefaultView(_bankEntities.Clients.Local);
            OffersViewSource.Source = Offers;


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
