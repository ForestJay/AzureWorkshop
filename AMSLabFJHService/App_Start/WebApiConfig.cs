using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;

namespace AMSLabFJHService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Database.SetInitializer(new AMSLabFJHInitializer());
        }
    }

    public class AMSLabFJHInitializer : ClearDatabaseSchemaIfModelChanges<AMSLabFJHContext>
    {

        protected override void Seed(AMSLabFJHContext context)
        {
            List<Person> people = new List<Person>
{
  new Person { Id = "1", Name = "Arthur Pewty", ExternalId = "Twitter:1234" },
  new Person { Id = "2", Name = "Arthur Dent", ExternalId = "Facebook:42" },
  new Person { Id = "3", Name = "Arthur Nudge", ExternalId = "Twitter:998" },
  new Person { Id = "4", Name = "Arthur Belling", ExternalId = "Facebook:321" },
 new Person { Id = "5", Name = "Zaphod Beeblebrox", ExternalId = "Facebook:123" },
  new Person { Id = "6", Name = "Tricia McMillan", ExternalId = "Facebook:124" }

};
            Add(context, people);

            List<Group> groups = new List<Group>
{
  new Group { Id = "1", Name = "Family", OwnerId = "1" },
  new Group { Id = "2", Name = "Friends", OwnerId = "2" }
};
            Add(context, groups);

            List<GroupMembership> memberships = new List<GroupMembership>
{
  new GroupMembership { Id = "1", GroupId = "1", PersonId = "3" },
  new GroupMembership { Id = "2", GroupId = "1", PersonId = "4" },
  new GroupMembership { Id = "3", GroupId = "2", PersonId = "5" }
};
            Add(context, memberships);

            List<CheckIn> checkIns = new List<CheckIn>
{
  new CheckIn { Id = "1", PersonId = "3", 
CheckInTime = "2014-09-02T07:16:04.2013330Z", 
Location = "51.6378317,-0.1509554" },
  new CheckIn { Id = "2", PersonId = "6", 
CheckInTime = "2014-09-02T07:16:04.2013330Z", 
Location = "51.5401091,-0.1012392" }
};
            Add(context, checkIns);

            base.Seed(context);
        }
        private static void Add<T>(AMSLabFJHContext context, IEnumerable<T> items)
        where T : EntityData
        {
            foreach (T item in items)
            {
                context.Set<T>().Add(item);
            }
        }
    }
}

