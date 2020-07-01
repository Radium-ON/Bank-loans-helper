using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientInfo.Views;
using FirstFloor.ModernUI.Presentation;
using LoanHelper.Core.Interfaces;

namespace ClientInfo.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "новый клиент",
                Source = new Uri($"/ClientInfo;component/Views/{nameof(ClientInfoView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
