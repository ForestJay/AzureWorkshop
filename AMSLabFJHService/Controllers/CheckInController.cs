using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;
using System;
using System.Data.Entity;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using AMSLabFJHService.Authentication;

namespace AMSLabFJHService.Controllers
{
   [AuthorizeLevel(AuthorizationLevel.User)]
    public class CheckInController : TableController<CheckIn>
    {
       private AMSLabFJHContext context;

       protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<CheckIn>(context, Request, Services);
        }

        // GET tables/CheckIn
        public IQueryable<CheckIn> GetAllCheckIn()
        {
            return Query(); 
        }

        // POST tables/CheckIn/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostCheckIn(CheckIn item)
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            string personId = await context.People
                .Where(p => p.ExternalId == externalId)
                .Select(p => p.Id)
                .SingleOrDefaultAsync();
            item.PersonId = personId;
            item.CheckInTime = DateTime.UtcNow.ToString("o"); 
            CheckIn current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/CheckIn/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCheckIn(string id)
        {
             return DeleteAsync(id);
        }

    }
}