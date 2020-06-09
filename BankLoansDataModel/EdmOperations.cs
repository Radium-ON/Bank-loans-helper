using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLoansDataModel
{
    public static class EdmOperations
    {
        public static async Task InsertNewOfferBankAsync(Offer offer, Bank bank)
        {
            using (var context = new RGR_BankLoansEntities())
            {
                //add instances to context
                context.Banks.Add(bank);
                context.Offers.Add(offer);

                // add instance to navigation property
                offer.Banks.Add(bank);

                //call SaveChanges from context to confirm inserts
                await context.SaveChangesAsync();
            }
        }

        public static async Task InsertExistingOfferBankAsync(int offerId, int bankId)
        {
            using (var context = new RGR_BankLoansEntities())
            {

                var offer = new Offer { PK_OfferId = offerId };
                context.Offers.Add(offer);
                context.Offers.Attach(offer);

                var bank = new Bank { PK_RegNumber = bankId };
                context.Banks.Add(bank);
                context.Banks.Attach(bank);

                offer.Banks.Add(bank);

                await context.SaveChangesAsync();
            }
        }

        public static List<object> GetOffersByBankId(int bankId)
        {
            using (var context = new RGR_BankLoansEntities())
            {
                var result = (
                    // instance from context
                    from a in context.Banks
                        // instance from navigation property
                    from b in a.Offers
                        //join to bring useful data
                    join c in context.Offers on b.PK_OfferId equals c.PK_OfferId
                    where a.PK_RegNumber == bankId
                    select new
                    {
                        ID = c.PK_OfferId,
                        Name = c.Interest
                    }).ToList();

                return new List<object> {result};
            }
        }
    }
}
