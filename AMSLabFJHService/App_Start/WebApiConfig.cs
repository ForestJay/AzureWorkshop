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
            List<Person> todoItems = new List<Person>
    {
      new Person { Id = "1", Name = "Arthur Pewty", ExternalId = "Twitter:1234" },
      new Person { Id = "2", Name = "Arthur Dent", ExternalId = "Facebook:42" }
    };

            foreach (Person todoItem in todoItems)
            {
                context.Set<Person>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

