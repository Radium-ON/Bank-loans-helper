using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using ClientInfo.Properties;
using ClientInfo.ViewModels;
using ClientInfo.Views;
using LoanHelper.Core.Events;
using Prism.Events;
using Prism.Services.Dialogs;

namespace ClientInfo.ViewModels
{
    public class ClientViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        private string _firstName;
        private string _lastName;
        private string _passport;
        private string _tin;
        private int _age;
        private int _seniority;
        private decimal _salary;

        #endregion

        public ClientViewModel(IEventAggregator eventAggregator, IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            AddClientCommand = new DelegateCommand(async () => await AddClient(), CanAddClient)
                .ObservesProperty(() => IsValid);
        }

        private bool CanAddClient()
        {
            return IsValid;
        }

        private async Task AddClient()
        {
            var newclient = new Client
            {
                FirstName = FirstName,
                LastName = LastName,
                Passport = Passport,
                TIN = TIN,
                Age = Age,
                Seniority = Seniority,
                Salary = Salary
            };
            var clientExist = await _bankEntities.Clients.AnyAsync(c => c.Passport == newclient.Passport);
            if (!clientExist)
            {
                _bankEntities.Clients.Add(newclient);
                var count = await _bankEntities.SaveChangesAsync(CancellationToken.None);
                _eventAggregator.GetEvent<ClientAddedEvent>().Publish();
                ShowClientAddingNotification("Добавление клиента", $"Добавлено {count} записей.");

            }
            else
            {
                ShowClientAddingNotification("Клиент не добавлен", $"Клиент с паспортом {newclient.Passport} уже существует.");
            }
        }

        private void ShowClientAddingNotification(string title, string message)
        {
            _dialogService.ShowDialog("NotificationDialogWithOK", new DialogParameters
            {
                { "Message", $"{message}" },
                { "Title", $"{title}"}
            }, r => { });
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
            if (IsStringMissing(TIN) || !IsStringAllDigits(TIN) || TIN.Length != 12 || TIN.StartsWith("0"))
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
