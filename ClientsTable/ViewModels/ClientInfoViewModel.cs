using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using ClientsTable.Properties;
using LoanHelper.Core.Extensions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ClientsTable.ViewModels
{
    public class ClientInfoViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;
        private readonly Client _client;

        #endregion

        public ClientInfoViewModel(Client client, IBankEntitiesContext bankEntities)
        {
            _client = client;
            _bankEntities = bankEntities;
        }

        public Client Client => _client;

        public bool IsClientUnique => !CheckIsClientContainsInContext(_client);

        private bool CheckIsClientContainsInContext(Client client)
        {
            return _bankEntities.Clients.Any(c => c.Passport == client.Passport || c.TIN == client.TIN);
        }

        #region Entity Properties

        public int ClientId => _client.PK_ClientId;

        public string FirstName
        {
            get => _client.FirstName;
            set
            {
                if (value == _client.FirstName)
                {
                    return;
                }

                _client.FirstName = value;

                RaisePropertyChanged(nameof(FirstName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string LastName
        {
            get => _client.LastName;
            set
            {
                if (value == _client.LastName)
                {
                    return;
                }

                _client.LastName = value;

                RaisePropertyChanged(nameof(LastName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Passport
        {
            get => _client.Passport;
            set
            {
                if (value == _client.Passport)
                {
                    return;
                }

                _client.Passport = value;

                RaisePropertyChanged(nameof(Passport));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Tin
        {
            get => _client.TIN;
            set
            {
                if (value == _client.TIN)
                {
                    return;
                }

                _client.TIN = value;

                RaisePropertyChanged(nameof(Tin));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public int Age
        {
            get => _client.Age;
            set
            {
                if (value == _client.Age)
                {
                    return;
                }

                _client.Age = value;

                RaisePropertyChanged(nameof(Age));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public int Seniority
        {
            get => _client.Seniority;
            set
            {
                if (value == _client.Seniority)
                {
                    return;
                }

                _client.Seniority = value;

                RaisePropertyChanged(nameof(Seniority));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public decimal Salary
        {
            get => _client.Salary;
            set
            {
                if (value == _client.Salary)
                {
                    return;
                }

                _client.Salary = value;

                RaisePropertyChanged(nameof(Salary));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<LoanAgreement> LoanAgreements => _client.LoanAgreements;
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
            "FirstName",
            "LastName",
            "Passport",
            "Tin",
            "Age",
            "Seniority",
            "Salary"
        };



        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "FirstName" => ValidateFirstName(),
                "LastName" => ValidateLastName(),
                "Passport" => ValidatePassport(),
                "Tin" => ValidateTin(),
                "Age" => ValidateAge(),
                "Seniority" => ValidateSeniority(),
                "Salary" => ValidateSalary(),
                _ => null
            };

            return error;
        }

        private string ValidateSalary()
        {
            if (Salary <= 0)
            {
                return Resources.client_error_missing_salary;
            }
            return null;
        }

        private string ValidateSeniority()
        {
            if (Seniority < 0 || Seniority > 100 || Seniority >= Age)
            {
                return Resources.client_error_missing_seniority;
            }
            return null;
        }

        private string ValidateAge()
        {
            if (Age <= 0 || Age <= Seniority)
            {
                return Resources.client_error_missing_age;
            }
            return null;
        }

        private string ValidateTin()
        {
            if (IsStringMissing(Tin) || !IsStringAllDigits(Tin) || Tin.Length != 12 || Tin.StartsWith("0"))
            {
                return Resources.client_error_missing_tin;
            }
            return null;
        }

        private string ValidatePassport()
        {
            if (IsStringMissing(Passport) || !IsStringAllDigits(Passport) || Passport.Length != 10 || Passport.StartsWith("0"))
            {
                return Resources.client_error_missing_passport;
            }
            return null;
        }

        private string ValidateFirstName()
        {
            if (IsStringMissing(FirstName) || !IsStringAllLetters(FirstName))
            {
                return Resources.client_error_missing_first_name;
            }
            return null;
        }

        private string ValidateLastName()
        {

            if (IsStringMissing(LastName) || !IsStringAllLetters(LastName))
                return Resources.client_error_missing_lastname;

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
