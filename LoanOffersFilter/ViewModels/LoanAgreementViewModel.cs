using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Mvvm;

namespace LoanOffersFilter.ViewModels
{
    public class LoanAgreementViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;
        private readonly LoanAgreement _agreement;

        #endregion

        public LoanAgreementViewModel(LoanAgreement agreement, IBankEntitiesContext bankEntities)
        {
            _agreement = agreement;
            _bankEntities = bankEntities;
        }

        public LoanAgreement Agreement => _agreement;

        public bool IsLoanAgreementUnique => !CheckIsClientContainsInContext(_agreement);

        private bool CheckIsClientContainsInContext(LoanAgreement agreement)
        {
            return _bankEntities.LoanAgreements.Any(l => l.AgreementNumber == agreement.AgreementNumber);
        }

        #region Entity Properties

        public int AgreementId => _agreement.PK_AgreementId;

        public bool? IsRepaid => _agreement.IsRepaid;

        public Bank Bank => _agreement.Bank;

        public Client Client => _agreement.Client;

        public string AgreementNumber
        {
            get => _agreement.AgreementNumber;
            set
            {
                if (value == _agreement.AgreementNumber)
                {
                    return;
                }

                _agreement.AgreementNumber = value;

                RaisePropertyChanged(nameof(AgreementNumber));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public DateTime ContractDate
        {
            get => _agreement.ContractDate;
            set
            {
                if (value == _agreement.ContractDate)
                {
                    return;
                }

                _agreement.ContractDate = value;

                RaisePropertyChanged(nameof(ContractDate));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal? Payment
        {
            get => _agreement.Payment;
            set
            {
                if (value == _agreement.Payment)
                {
                    return;
                }

                _agreement.Payment = value;

                RaisePropertyChanged(nameof(Payment));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal LoanAmount
        {
            get => _agreement.LoanAmount;
            set
            {
                if (value == _agreement.LoanAmount)
                {
                    return;
                }

                _agreement.LoanAmount = value;

                RaisePropertyChanged(nameof(LoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int Months
        {
            get => _agreement.Months;
            set
            {
                if (value == _agreement.Months)
                {
                    return;
                }

                _agreement.Months = value;

                RaisePropertyChanged(nameof(Months));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public float Interest
        {
            get => _agreement.Interest;
            set
            {
                if (value == _agreement.Interest)
                {
                    return;
                }

                _agreement.Interest = value;

                RaisePropertyChanged(nameof(Interest));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        #endregion

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => GetValidationError(columnName);

        public string Error => null;

        #endregion

        #region Validation

        public bool IsValid
        {
            get
            {
                return _validatedProperties.All(property => GetValidationError(property) == null);
            }
        }

        private static readonly string[] _validatedProperties =
        {
            "AgreementNumber",
            "ContractDate"
        };



        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "AgreementNumber" => ValidateAgreementNumber(),
                "ContractDate" => ValidateContractDate(),
                _ => null
            };

            return error;
        }

        private string ValidateAgreementNumber()
        {
            if (IsStringMissing(AgreementNumber) || !IsStringAllDigits(AgreementNumber))
            {
                return "Номер договора состоит только из цифр.";
            }
            return null;
        }

        private string ValidateContractDate()
        {

            if (ContractDate.Date > DateTime.Today)
                return "Установите дату не позже сегодняшней.";

            return null;
        }

        private static bool IsStringMissing(string value)
        {
            return
                string.IsNullOrEmpty(value) ||
                value.Trim() == string.Empty;
        }

        private static bool IsStringAllDigits(string value)
        {
            return value.All(char.IsDigit);
        }
        
        #endregion
    }
}
