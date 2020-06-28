﻿using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using LoanHelper.Core;
using LoanHelper.Core.Events;
using LoanHelper.Core.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace LoanHelper.ViewModels
{
    public class AllClientsViewModel : ModernViewModelBase
    {
        private readonly IBankEntitiesContext _bankEntities;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

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
            var hasChanges = (_bankEntities as DbContext)?.ChangeTracker.HasChanges();

            if (hasChanges == true)
            {
                _dialogService.ShowDialog(nameof(OkCancelDialog), new DialogParameters
                {
                    { "Message", "Все внесённые изменения будут сохранены.\nГотовы продолжить?" },
                    { "Title", "Остановитесь"}
                }, r =>
                {
                    if (r.Result == ButtonResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                });
            }
            Debug.WriteLine("AllClientsViewModel - NavigatingFrom");
        }
    }
}
