using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Extensions;
using BankLoansDataModel.Services;
using OffersTable.Properties;
using Prism.Events;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferInfoViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;
        private readonly Offer _offer;

        private bool _isSelected;

        #endregion

        public OfferInfoViewModel(Offer offer, IBankEntitiesContext bankEntities)
        {
            _offer = offer;
            _bankEntities = bankEntities;
        }

        public Offer Offer => _offer;

        public bool IsOfferUnique => !CheckIsOfferContainsInContext(_offer);

        /// <summary>
        /// Определяет, содержится ли в контексте элемент <see cref="Offer"/> с заданными атрибутами.
        /// </summary>
        /// <param name="offer">Новая сущность "Предложение банка"</param>
        /// <returns>True: предложение с параметрами найдено в контексте. False: предложение уникально.</returns>
        private bool CheckIsOfferContainsInContext(Offer offer)
        {
            var findedOffer = _bankEntities.Offers.FirstOrDefault(existedOffer =>
                    existedOffer.Interest == offer.Interest &&
                    existedOffer.MinLoanAmount == offer.MinLoanAmount &&
                    existedOffer.MaxLoanAmount == offer.MaxLoanAmount &&
                    existedOffer.MaxOfMonths == offer.MaxOfMonths &&
                    existedOffer.ActiveLoansNumber == offer.ActiveLoansNumber &&
                    existedOffer.MinSeniority == offer.MinSeniority &&
                    existedOffer.MinAge == offer.MinAge);

            return findedOffer != null;
        }

        #region Entity Properties

        public int OfferId => _offer.PK_OfferId;

        public float Interest
        {
            get => _offer.Interest;
            set
            {
                if (value == _offer.Interest)
                {
                    return;
                }

                _offer.Interest = (float) (value*0.01);

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
            "MinAge",
            "ActiveLoansNumber"
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
                "ActiveLoansNumber" => ValidateActiveLoansNumber(),
                _ => null
            };
            return error;
        }

        private string ValidateActiveLoansNumber()
        {
            if (ActiveLoansNumber<0)
            {
                return Resources.offer_error_active_loans_number_out_of_range;
            }
            return null;
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
            if (Interest <= 0f)
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
