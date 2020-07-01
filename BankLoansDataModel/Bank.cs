//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace BankLoansDataModel
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Bank
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bank()
        {
            this.LoanAgreements = new ObservableCollection<LoanAgreement>();
            this.Offers = new ObservableCollection<Offer>();
        }
    
        public int PK_RegNumber { get; set; }
        public string Name { get; set; }

        [MinLength(13)]
        public string OGRN { get; set; }
        public string License { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<LoanAgreement> LoanAgreements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Offer> Offers { get; set; }
    }
}
