using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace AMSLabFJHService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]

    public class GroupController : TableController<Group>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<Group>(context, Request, Services);
        }

        // GET tables/Group
        public IQueryable<Group> GetAllGroup()
        {
            return Query(); 
        }

        // GET tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Group> GetGroup(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Group> PatchGroup(string id, Delta<Group> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostGroup(Group item)
        {
            Group current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroup(string id)
        {
             return DeleteAsync(id);
        }

    }
}