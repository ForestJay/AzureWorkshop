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
    public class InvitationController : TableController<Invitation>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<Invitation>(context, Request, Services);
        }

        // GET tables/Invitation
        public IQueryable<Invitation> GetAllInvitation()
        {
            return Query(); 
        }

        // GET tables/Invitation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Invitation> GetInvitation(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Invitation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Invitation> PatchInvitation(string id, Delta<Invitation> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Invitation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostInvitation(Invitation item)
        {
            Invitation current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Invitation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteInvitation(string id)
        {
             return DeleteAsync(id);
        }

    }
}