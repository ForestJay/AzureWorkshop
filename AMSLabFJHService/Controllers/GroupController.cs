using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using AMSLabFJHService.Authentication;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;

namespace AMSLabFJHService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class GroupController : TableController<Group>
    {
        private AMSLabFJHContext context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<Group>(context, Request, Services);
        }

        // GET tables/Group
        public async Task<IQueryable<Group>> GetAllGroup()
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            return
                from p in context.People
                join g in context.Groups on p.Id equals g.OwnerId
                where p.ExternalId == externalId
                select g;
        }

        // GET tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<SingleResult<Group>> GetGroup(string id)
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            var q =
                from p in context.People
                join g in context.Groups on p.Id equals g.OwnerId
                where p.ExternalId == externalId && g.Id == id
                select g;
            return new SingleResult<Group>(q);
        }

        // POST tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostGroup(Group item)
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            string personId = await context.People
                .Where(p => p.ExternalId == externalId)
                .Select(p => p.Id)
                .SingleOrDefaultAsync();
            item.OwnerId = personId;
            Group current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
    }
}