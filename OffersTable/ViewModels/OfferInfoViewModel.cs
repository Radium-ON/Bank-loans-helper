using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Events;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferInfoViewModel : BindableBase
    {
        #region Backing Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;
        private readonly Offer _offer;

        private float _interest;
        private decimal _minLoanAmount;
        private decimal _maxLoanAmount;
        private int _maxOfMonths;
        private int? _activeLoansNumber;
        private int? _minSeniority;
        private int? _minAge;

        #endregion

        public OfferInfoViewModel(Offer offer, IDialogService dialogService, IBankEntitiesContext bankEntities, IEventAggregator eventAggregator)
        {
            _offer = offer;
            _dialogService = dialogService;
            _bankEntities = bankEntities;
            _eventAggregator = eventAggregator;
        }

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
            }
        }

        public decimal MinLoanAmount
        {
            get => _minLoanAmount;
            set => _minLoanAmount = value;
        }

        public decimal MaxLoanAmount
        {
            get => _maxLoanAmount;
            set => _maxLoanAmount = value;
        }

        public int MaxOfMonths
        {
            get => _maxOfMonths;
            set => _maxOfMonths = value;
        }

        public Nullable<int> ActiveLoansNumber
        {
            get => _activeLoansNumber;
            set => _activeLoansNumber = value;
        }

        public Nullable<int> MinSeniority
        {
            get => _minSeniority;
            set => _minSeniority = value;
        }

        public Nullable<int> MinAge
        {
            get => _minAge;
            set => _minAge = value;
        }

    }
}
