using System;
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

        public ClientInfoViewModel(IEventAggregator eventAggregator, IBankEntitiesContext bankEntities, IDialogService dialogService)
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

            await _bankEntities.Clients.LoadAsync();
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
            var clientExist = await _bankEntities.Clients.AnyAsync(c => c.Passport == newclient.Passport || c.TIN == newclient.TIN);
            if (!clientExist)
            {
                _bankEntities.Clients.Add(newclient);
                var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                var message = "Новый клиент добавлен успешно.";
                if (!status.IsValid)
                {
                    message = string.Join("\n", status.EfErrors);
                }
                ShowClientAddingNotification("Добавление клиента", message);

            }
            else
            {
                ShowClientAddingNotification("Клиент не добавлен", $"Клиент с паспортом {newclient.Passport} и ИНН {newclient.TIN} уже существует.");
            }
        }

        private void ShowClientAddingNotification(string title, string message)
        {
            _dialogService.ShowOkDialog(title, message, r => { });
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

            var error = propertyName switch
            {
                "FirstName" => ValidateFirstName(),
                "LastName" => ValidateLastName(),
                "Passport" => ValidatePassport(),
                "TIN" => ValidateTin(),
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
