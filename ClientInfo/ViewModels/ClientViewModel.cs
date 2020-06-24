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

namespace ClientInfo.ViewModels
{
    public class ClientViewModel : BindableBase, IDataErrorInfo
    {
        #region Backing Fields
        private string _firstName;
        private string _surName;
        private string _passport;
        private string _tin;
        private int _age;
        private int _seniority;
        private decimal _salary;

        #endregion
        public ClientViewModel()
        {
        }

        #region Bindable Props

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string Surname
        {
            get => _surName;
            set => SetProperty(ref _surName, value);
        }
        public string Passport
        {
            get { return _passport; }
            set { SetProperty(ref _passport, value); }
        }
        public string TIN
        {
            get { return _tin; }
            set { SetProperty(ref _tin, value); }
        }
        public int Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }
        public int Seniority
        {
            get { return _seniority; }
            set { SetProperty(ref _seniority, value); }
        }
        public decimal Salary
        {
            get { return _salary; }
            set { SetProperty(ref _salary, value); }
        }
        #endregion

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => GetValidationError(columnName);

        public string Error { get => null; }

        #endregion

        #region Validation

        public bool IsValid
        {
            get
            {
                foreach (string property in _validatedProperties)
                    if (GetValidationError(property) != null)
                        return false;

                return true;
            }
        }

        static readonly string[] _validatedProperties =
        {
            "FirstName",
            "LastName",
            "Passport",
            "TIN",
            "Age",
            "Seniority",
            "Salary"
        };



        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "FirstName":
                    error = ValidateFirstName();
                    break;

                case "Surname":
                    error = ValidateSurName();
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
            if (Salary<=0)
            {
                return Resources.client_error_missing_salary;
            }
            return null;
        }

        private string ValidateSeniority()
        {
            if (Seniority>100)
            {
                return Resources.client_error_missing_seniority;
            }
            return null;
        }

        private string ValidateAge()
        {
            if (Age==0)
            {
                return Resources.client_error_missing_age;
            }
            return null;
        }

        private string ValidateTin()
        {
            if (IsStringMissing(TIN))
            {
                return Resources.client_error_missing_tin;
            }
            return null;
        }

        private string ValidatePassport()
        {
            if (IsStringMissing(Passport))
            {
                return Resources.client_error_missing_passport;
            }
            return null;
        }

        private string ValidateFirstName()
        {
            if (IsStringMissing(FirstName))
            {
                return Resources.client_error_missing_first_name;
            }
            return null;
        }

        private string ValidateSurName()
        {

            if (IsStringMissing(Surname))
                return Resources.client_error_missing_surname;

            return null;
        }

        private static bool IsStringMissing(string value)
        {
            return
                string.IsNullOrEmpty(value) ||
                value.Trim() == string.Empty;
        }
        #endregion
    }
}
