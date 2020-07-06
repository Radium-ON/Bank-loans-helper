using System;
using System.ComponentModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BanksTable.ViewModels
{
    public class BankAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addBankCommand;
        public DelegateCommand AddBankCommand =>
            _addBankCommand ??= new DelegateCommand(AddClientToContext, CanAddClient)
                .ObservesProperty(() => BankInfoViewModel.IsValid)
                .ObservesProperty(() => BankInfoViewModel.IsBankUnique);
        
        #endregion

        #region Bindable Properties

        private string _title = "Новый банк";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private BankInfoViewModel _bankInfoViewModel;
        public BankInfoViewModel BankInfoViewModel
        {
            get => _bankInfoViewModel;
            set => SetProperty(ref _bankInfoViewModel, value);
        }

        #endregion

        private void AddClientToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedBankViewModel", BankInfoViewModel } }));
        }

        private bool CanAddClient()
        {
            return BankInfoViewModel == null || BankInfoViewModel.IsValid && BankInfoViewModel.IsBankUnique;
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
            BankInfoViewModel = parameters.GetValue<BankInfoViewModel>(nameof(BankInfoViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (BankInfoViewModel as IDataErrorInfo)[columnName];

        public string Error => (BankInfoViewModel as IDataErrorInfo).Error;

        #endregion
    }
}
