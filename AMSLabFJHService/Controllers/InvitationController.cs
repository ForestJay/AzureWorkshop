using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;
using AMSLabFJHService.Authentication;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Web;   
using System.Data.Entity;

namespace AMSLabFJHService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]

    public class InvitationController : TableController<Invitation>
    {
        private AMSLabFJHContext context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<Invitation>(context, Request, Services);
        }

        // POST tables/Invitation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostInvitation(Invitation item)
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            var q =
               from g in context.Groups
               where g.Id == item.GroupId
               join owner in context.People on g.OwnerId equals owner.Id
               select owner.ExternalId;

            string groupOwnerExternalId = await q.SingleOrDefaultAsync();
            if (groupOwnerExternalId != externalId)
            {
                throw new HttpException(403, "Forbidden");
            }
            Invitation current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
    }
}