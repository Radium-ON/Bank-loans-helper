using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using LoanHelper.Core;
using LoanHelper.Core.Events;
using Prism.Commands;
using Prism.Events;

namespace LoanHelper.ViewModels
{
    public class AllClientsViewModel : ModernViewModelBase
    {
        private readonly IBankEntitiesContext _bankEntities;
        private readonly IEventAggregator _eventAggregator;

        public AllClientsViewModel(IBankEntitiesContext bankEntities, IEventAggregator eventAggregator)
        {
            _bankEntities = bankEntities;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ClientAddedEvent>().Subscribe(OnClientAdded);

            Clients = new AsyncObservableCollection<Client>();

            NavigatingFromCommand = new DelegateCommand(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        private void OnClientAdded()
        {
            _bankEntities.Clients.Load();
        }



        #region Backing Fields
        private ObservableCollection<Client> _clients;



        #endregion

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
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
        private async Task LoadDataAsync()
        {
            await _bankEntities.Clients.LoadAsync();
            Clients = _bankEntities.Clients.Local;
            Debug.WriteLine("AllClientsViewModel - LoadDataAsync");
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
