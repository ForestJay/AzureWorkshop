using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;
using System.Data.Entity;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using AMSLabFJHService.Authentication;

namespace AMSLabFJHService.Controllers
{
    public class PersonController : TableController<Person>
    {
        [AuthorizeLevel(AuthorizationLevel.User)]

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            AMSLabFJHContext context = new AMSLabFJHContext();
            DomainManager = new EntityDomainManager<Person>(context, Request, Services);
        }

        // GET tables/Person
        public async Task<IQueryable<Person>> GetAllPerson()
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            return DomainManager.Query().Where(p => p.ExternalId == externalId);
        }

        // GET tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Person> GetPerson(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<Person> PatchPerson(string id, Delta<Person> patch)
        {
            string externalId = await UserInformation.ExternalIdFromUser(User);
            Person person =
              await DomainManager.Query().Where(p => p.Id == id).SingleOrDefaultAsync();

            if (person != null && person.ExternalId != externalId)
            {
                throw new HttpException(403, "Forbidden");
            }
            return await UpdateAsync(id, patch);
        }

        // POST tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPerson(Person item)
        {
            item.ExternalId = await UserInformation.ExternalIdFromUser(User);
            Person current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

    }
}