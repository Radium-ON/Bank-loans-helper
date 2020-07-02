using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using OffersTable.Properties;
using Prism.Events;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferInfoViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        //        private readonly IEventAggregator _eventAggregator;
        //        private readonly IBankEntitiesContext _bankEntities;
        //        private readonly IDialogService _dialogService;
        private readonly Offer _offer;

        private bool _isSelected;

        #endregion

        public OfferInfoViewModel(Offer offer)
        {
            _offer = offer;
            //            _dialogService = dialogService;
            //            _bankEntities = bankEntities;
            //            _eventAggregator = eventAggregator;
        }

        public Offer GetOffer() => _offer;

        #region Entity Properties

        public int OfferId => _offer.PK_OfferId;

        public float Interest
        {
            get => _offer.Interest;
            set
            {
                if (Math.Abs(value - _offer.Interest) < 1)
                {
                    return;
                }

                _offer.Interest = value;

                RaisePropertyChanged(nameof(Interest));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal MinLoanAmount
        {
            get => _offer.MinLoanAmount;
            set
            {
                if (value == _offer.MinLoanAmount)
                {
                    return;
                }

                _offer.MinLoanAmount = value;

                RaisePropertyChanged(nameof(MinLoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal MaxLoanAmount
        {
            get => _offer.MaxLoanAmount;
            set
            {
                if (value == _offer.MaxLoanAmount)
                {
                    return;
                }

                _offer.MaxLoanAmount = value;

                RaisePropertyChanged(nameof(MaxLoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int MaxOfMonths
        {
            get => _offer.MaxOfMonths;
            set
            {
                if (value == _offer.MaxOfMonths)
                {
                    return;
                }

                _offer.MaxOfMonths = value;

                RaisePropertyChanged(nameof(MaxOfMonths));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? ActiveLoansNumber
        {
            get => _offer.ActiveLoansNumber;
            set
            {
                if (value == _offer.ActiveLoansNumber)
                {
                    return;
                }

                _offer.ActiveLoansNumber = value;

                RaisePropertyChanged(nameof(ActiveLoansNumber));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? MinSeniority
        {
            get => _offer.MinSeniority;
            set
            {
                if (value == _offer.MinSeniority)
                {
                    return;
                }

                _offer.MinSeniority = value;

                RaisePropertyChanged(nameof(MinSeniority));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? MinAge
        {
            get => _offer.MinAge;
            set
            {
                if (value == _offer.MinAge)
                {
                    return;
                }

                _offer.MinAge = value;

                RaisePropertyChanged(nameof(MinAge));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<Bank> Banks => _offer.Banks;

        #endregion

        #region UI Properties

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        #endregion

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => GetValidationError(columnName);

        public string Error => null;

        #endregion

        #region Validation

        public bool IsValid => _validatedProperties.All(property => GetValidationError(property) == null);

        private static readonly string[] _validatedProperties =
        {
            "Interest",
            "MinLoanAmount",
            "MaxLoanAmount",
            "MinSeniority",
            "MinAge"
        };



        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "Interest" => ValidateInterest(),
                "MinLoanAmount" => ValidateMinLoanAmount(),
                "MaxLoanAmount" => ValidateMaxLoanAmount(),
                "MinSeniority" => ValidateMinSeniority(),
                "MinAge" => ValidateMinAge(),
                _ => null
            };
            return error;
        }



        private string ValidateMinSeniority()
        {
            if (MinSeniority < 0 || MinSeniority > 100 || MinSeniority >= MinAge)
            {
                return Resources.offer_error_seniority_out_of_range;
            }
            return null;
        }

        private string ValidateMinAge()
        {
            if (MinAge <= 0 || MinAge <= MinSeniority)
            {
                return Resources.offer_error_age_out_of_range;
            }
            return null;
        }

        private string ValidateMaxLoanAmount()
        {
            if (MaxLoanAmount <= MinLoanAmount || MaxLoanAmount <= 0)
            {
                return Resources.offer_error_maxloanamount_out_of_range;
            }
            return null;
        }

        private string ValidateInterest()
        {
            if (Interest <= 0)
            {
                return Resources.offer_error_interest_negate;
            }
            return null;
        }

        private string ValidateMinLoanAmount()
        {

            if (MinLoanAmount >= MaxLoanAmount || MinLoanAmount <= 0)
            {
                return Resources.offer_error_minloanamount_out_of_range;
            }
            return null;
        }

        #endregion
    }
}
