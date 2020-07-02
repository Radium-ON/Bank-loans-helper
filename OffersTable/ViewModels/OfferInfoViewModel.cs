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
            get => _offer.MinLoanAmount;
            set
            {
                if (value == _offer.MinLoanAmount)
                {
                    return;
                }

                _offer.MinLoanAmount = value;

                RaisePropertyChanged(nameof(MinLoanAmount));
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
            }
        }

    }
}
