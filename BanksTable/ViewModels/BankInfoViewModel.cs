using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using BanksTable.Properties;
using Prism.Mvvm;

namespace BanksTable.ViewModels
{
    public class BankInfoViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;
        private readonly Bank _bank;

        #endregion

        public BankInfoViewModel(Bank bank, IBankEntitiesContext bankEntities)
        {
            _bank = bank;
            _bankEntities = bankEntities;
        }

        public Bank Bank => _bank;

        public bool IsBankUnique => !CheckIsBankContainsInContext(_bank);

        private bool CheckIsBankContainsInContext(Bank bank)
        {
            return _bankEntities.Banks.Any(c => c.OGRN == bank.OGRN);
        }

        #region Entity Properties

        public int BankId => _bank.PK_RegNumber;

        public string BankName
        {
            get => _bank.Name;
            set
            {
                if (value == _bank.Name)
                {
                    return;
                }

                _bank.Name = value;

                RaisePropertyChanged(nameof(BankName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public string Ogrn
        {
            get => _bank.OGRN;
            set
            {
                if (value == _bank.OGRN)
                {
                    return;
                }

                _bank.OGRN = value;

                RaisePropertyChanged(nameof(Ogrn));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public string BankLicense
        {
            get => _bank.License;
            set
            {
                if (value == _bank.License)
                {
                    return;
                }

                _bank.License = value;

                RaisePropertyChanged(nameof(BankLicense));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<LoanAgreement> LoanAgreements => _bank.LoanAgreements;

        public ObservableCollection<Offer> Offers => _bank.Offers;
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
            "BankName",
            "Ogrn"
        };



        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "BankName" => ValidateBankName(),
                "Ogrn" => ValidateOgrn(),
                _ => null
            };

            return error;
        }

        private string ValidateOgrn()
        {
            if (IsStringMissing(Ogrn) || !IsStringAllDigits(Ogrn) || Ogrn.Length != 13 || Ogrn.StartsWith("0"))
            {
                return Resources.bank_error_missing_ogrn;
            }
            return null;
        }

        private string ValidateLicense()
        {
            if (IsStringMissing(BankLicense))
            {
                return Resources.bank_error_missing_license;
            }
            return null;
        }

        private string ValidateBankName()
        {

            if (IsStringMissing(BankName))
                return Resources.bank_error_missing_bankname;

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

        private static bool IsStringAllLetters(string value)
        {
            return value.All(char.IsLetter);
        }

        #endregion
    }
}
