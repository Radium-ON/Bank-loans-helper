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
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core;
using LoanHelper.Core.Events;
using LoanHelper.Core.Extensions;
using LoanHelper.Core.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace LoanHelper.ViewModels
{
    public class AllClientsViewModel : ModernViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<Client> _clients;



        private readonly IBankEntitiesContext _bankEntities;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        #endregion

        public AllClientsViewModel(IBankEntitiesContext bankEntities, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            _eventAggregator.GetEvent<ClientAddedEvent>().Subscribe(OnClientAdded);

            Clients = new AsyncObservableCollection<Client>();

            UpdateClientCommand = new DelegateCommand<Client>(UpdateSelectedClient);
            DeleteClientCommand = new DelegateCommand<Client>(async client => await DeleteSelectedClient(client));

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        private async Task DeleteSelectedClient(Client client)
        {
            if (client != null)
            {
                _bankEntities.Clients.Remove(client);
                await _bankEntities.SaveChangesAsync(CancellationToken.None);
            }
        }

        private void UpdateSelectedClient(Client client)
        {


        }

        private void OnClientAdded()
        {
            _bankEntities.Clients.Load();
        }

        /// <summary>
        /// Возвращает список обновленных клиентов <see cref="Client"/>, чей <see cref="Client.Passport"/> или <see cref="Client.TIN"/> совпадает с базой; асинхронный.
        /// </summary>
        /// <param name="objectContext">Контекст объектов базы данных.</param>
        /// <returns>Список неуникальных клиентов.</returns>
        private async Task<List<Client>> GetNotUniqueClientsAsync(ObjectContext objectContext)
        {
            var updatedClients = GetModifiedClientsAsEnumerable(objectContext);

            var notUniqueClients = await updatedClients.AsAsyncQueryable()
                .Where(c => _bankEntities.Clients.Any(b => b.PK_ClientId != c.PK_ClientId && (b.Passport == c.Passport || b.TIN == c.TIN))).ToListAsync();
            return notUniqueClients;
        }

        private static IQueryable<Client> GetModifiedClientsAsEnumerable(ObjectContext objectContext)
        {
            var updatedObjects =
                from entry in objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).AsAsyncQueryable()
                where entry.EntityKey != null
                select entry.Entity as Client;
            return updatedObjects;
        }


        #region Properties

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        #endregion

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("AllClientsViewModel - VisibilityChanged");
        }

        #region Delegate Commands

        public DelegateCommand<Client> UpdateClientCommand { get; private set; }
        public DelegateCommand<Client> DeleteClientCommand { get; private set; }

        #endregion

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
        private void NavigatingFrom(NavigatingCancelEventArgs e)
        {
            var dbcontext = _bankEntities as DbContext;
            var hasChanges = dbcontext?.ChangeTracker.HasChanges();

            if (hasChanges == true)
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("clients_edited_dialog_title") as string,
                    Application.Current.FindResource("clients_edited_dialog_message") as string,
                    async r =>
                              {
                                  if (r.Result == ButtonResult.Cancel)
                                  {
                                      e.Cancel = true;
                                  }
                                  else
                                  {
                                      var badClients = await GetNotUniqueClientsAsync(CurrentObjectContext);

                                      if (badClients.Count == 0)
                                      {
                                          await _bankEntities.SaveChangesAsync(CancellationToken.None);
                                      }
                                      else
                                      {
                                          var passports = badClients.Select(badClient => badClient.Passport).ToList();
                                          var tins = badClients.Select(b => b.TIN).ToList();
                                          _dialogService.ShowOkDialog(
                                              "Нельзя перейти",
                                              $"Клиенты с данными паспорта: {string.Join(", ", passports)} и ИНН: {string.Join(", ", tins)} уже существуют.",
                                              async n =>
                                              {
                                                  e.Cancel = true;
                                                  foreach (var badClient in badClients)
                                                  {
                                                      await dbcontext?.ReloadEntityAsync(badClient);
                                                  }
                                              });
                                      }
                                  }
                              });
            }
            Debug.WriteLine("AllClientsViewModel - NavigatingFrom");
        }
    }
}
