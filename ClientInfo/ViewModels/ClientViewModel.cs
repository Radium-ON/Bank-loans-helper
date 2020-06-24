using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientInfo.Properties;
using Prism.Events;

namespace ClientInfo.ViewModels
{
    public class ClientViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields
        private readonly IEventAggregator _eventAggregator;

        private string _firstName;
        private string _lastName;
        private string _passport;
        private string _tin;
        private int _age;
        private int _seniority;
        private decimal _salary;

        private bool _isValid;
        #endregion

        public ClientViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            AddClientCommand = new DelegateCommand(AddClient, CanAddClient).ObservesProperty(() => IsValid);
        }

        private bool CanAddClient()
        {
            return IsValid;
        }

        private void AddClient()
        {
            FirstName = "d";
        }

        #region Bindable Properties

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public string Passport
        {
            get { return _passport; }
            set
            {
                SetProperty(ref _passport, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public string TIN
        {
            get { return _tin; }
            set
            {
                SetProperty(ref _tin, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public int Age
        {
            get { return _age; }
            set
            {
                SetProperty(ref _age, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public int Seniority
        {
            get { return _seniority; }
            set
            {
                SetProperty(ref _seniority, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        public decimal Salary
        {
            get { return _salary; }
            set
            {
                SetProperty(ref _salary, value, () => RaisePropertyChanged(nameof(IsValid)));
            }
        }
        #endregion

        #region DelegateCommands

        public DelegateCommand AddClientCommand { get; private set; }

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
            "TIN",
            "Age",
            "Seniority",
            "Salary"
        };



        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "FirstName":
                    error = ValidateFirstName();
                    break;

                case "LastName":
                    error = ValidateLastName();
                    break;

                case "Passport":
                    error = ValidatePassport();
                    break;

                case "TIN":
                    error = ValidateTin();
                    break;

                case "Age":
                    error = ValidateAge();
                    break;

                case "Seniority":
                    error = ValidateSeniority();
                    break;

                case "Salary":
                    error = ValidateSalary();
                    break;
            }

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
            if (Seniority < 0 || Seniority > 100)
            {
                return Resources.client_error_missing_seniority;
            }
            return null;
        }

        private string ValidateAge()
        {
            if (Age <= 0)
            {
                return Resources.client_error_missing_age;
            }
            return null;
        }

        private string ValidateTin()
        {
            if (IsStringMissing(TIN) || !IsStringAllDigits(TIN) || TIN.Length != 12)
            {
                return Resources.client_error_missing_tin;
            }
            return null;
        }

        private string ValidatePassport()
        {
            if (IsStringMissing(Passport) || !IsStringAllDigits(Passport) || Passport.Length != 10)
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
