using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ClientsTable.ViewModels
{
    public class ClientAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addClientCommand;
        public DelegateCommand AddClientCommand =>
            _addClientCommand ??= new DelegateCommand(AddClientToContext, CanAddClient)
                .ObservesProperty(() => ClientInfoViewModel.IsValid)
                .ObservesProperty(() => ClientInfoViewModel.IsClientUnique);
        
        #endregion

        #region Bindable Properties

        private string _title = "Новый клиент";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ClientInfoViewModel _clientInfoViewModel;
        public ClientInfoViewModel ClientInfoViewModel
        {
            get => _clientInfoViewModel;
            set => SetProperty(ref _clientInfoViewModel, value);
        }

        #endregion

        private void AddClientToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedClientViewModel", ClientInfoViewModel } }));
        }

        private bool CanAddClient()
        {
            return ClientInfoViewModel == null || ClientInfoViewModel.IsValid && ClientInfoViewModel.IsClientUnique;
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            var result = parameter?.ToLower() switch
            {
                "true" => ButtonResult.OK,
                "false" => ButtonResult.Cancel,
                _ => ButtonResult.None
            };

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            ClientInfoViewModel = parameters.GetValue<ClientInfoViewModel>(nameof(ClientInfoViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (ClientInfoViewModel as IDataErrorInfo)[columnName];

        public string Error => (ClientInfoViewModel as IDataErrorInfo).Error;

        #endregion
    }
}
