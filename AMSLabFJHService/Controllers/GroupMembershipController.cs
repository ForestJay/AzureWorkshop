using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;

namespace AMSLabFJHService.Controllers
{
    public class GroupMembershipController : TableController<GroupMembership>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<GroupMembership>(context, Request, Services);
        }

        // GET tables/GroupMembership
        public IQueryable<GroupMembership> GetAllGroupMembership()
        {
            return Query(); 
        }

        // GET tables/GroupMembership/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<GroupMembership> GetGroupMembership(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/GroupMembership/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<GroupMembership> PatchGroupMembership(string id, Delta<GroupMembership> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/GroupMembership/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostGroupMembership(GroupMembership item)
        {
            GroupMembership current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/GroupMembership/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroupMembership(string id)
        {
             return DeleteAsync(id);
        }

    }
}