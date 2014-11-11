using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace AMSLabFJHService.Authentication
{
    public class UserInformation
    {
        public static async Task<string> ExternalIdFromUser(IPrincipal principal)
        {
            var user = (ServiceUser)principal;
            Collection<ProviderCredentials> allCredentials =
                await user.GetIdentitiesAsync();
            if (allCredentials != null &&
                allCredentials.Count > 0)
            {
                ProviderCredentials credentials =
                    allCredentials[0];
                var aadCredentials =
                    credentials as AzureActiveDirectoryCredentials;
                return aadCredentials != null
                    ? string.Format("Aad:{0},{1}",
                    aadCredentials.TenantId, aadCredentials.ObjectId)
                    : credentials.UserId;
            }

            return null;
        }
    }
}