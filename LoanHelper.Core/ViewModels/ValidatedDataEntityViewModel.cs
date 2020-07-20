using System;
using System.ComponentModel;
using System.Linq;

namespace LoanHelper.Core.ViewModels
{
    public abstract class ValidatedDataEntityViewModel<TEntity> : IDataErrorInfo
    {
        #region Backing Fields

        private protected readonly TEntity _entity;

        private readonly string[] _validatedProperties;

        #endregion

        protected ValidatedDataEntityViewModel(TEntity entity, string[] validatedProperties)
        {
            _entity = entity;
            _validatedProperties = validatedProperties;
        }

        public TEntity Entity => _entity;

        public bool IsUnique => !CheckIsEntityContainsInContext(_entity);

        protected abstract bool CheckIsEntityContainsInContext(TEntity entity);



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

        public string[] ValidatedProperties => _validatedProperties;



        protected virtual string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(_validatedProperties, propertyName) < 0)
                return null;

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
