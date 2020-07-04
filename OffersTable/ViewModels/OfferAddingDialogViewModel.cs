using System;
using System.ComponentModel;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

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


        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            var result = parameter?.ToLower() switch
            {
                "true" => ButtonResult.OK,
                "false" => ButtonResult.Cancel,
                _ => ButtonResult.None
            };

            RaiseRequestClose(new DialogResult(result, new DialogParameters { { "AddedOffer", OfferInfoViewModel.GetOffer() } }));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return OfferInfoViewModel.IsValid;
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