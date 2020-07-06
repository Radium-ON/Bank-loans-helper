using System;
using System.ComponentModel;
using System.Diagnostics;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addOfferCommand;
        public DelegateCommand AddOfferCommand =>
            _addOfferCommand ??= new DelegateCommand(AddOfferToContext, CanAddOffer)
                .ObservesProperty(() => OfferInfoViewModel.IsValid)
                .ObservesProperty(() => OfferInfoViewModel.IsOfferUnique);

        private bool CanAddOffer()
        {
            return OfferInfoViewModel == null || OfferInfoViewModel.IsValid && OfferInfoViewModel.IsOfferUnique;
        }

        #endregion

        #region Bindable Properties

        private string _title = "Новое предложение";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private OfferInfoViewModel _offerInfoViewModel;
        public OfferInfoViewModel OfferInfoViewModel
        {
            get => _offerInfoViewModel;
            set => SetProperty(ref _offerInfoViewModel, value);
        }

        #endregion

        private void AddOfferToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedOfferViewModel", OfferInfoViewModel } }));
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
            OfferInfoViewModel = parameters.GetValue<OfferInfoViewModel>(nameof(OfferInfoViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (OfferInfoViewModel as IDataErrorInfo)[columnName];

        public string Error => (OfferInfoViewModel as IDataErrorInfo).Error;

        #endregion
    }
}