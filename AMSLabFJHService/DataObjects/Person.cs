using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AMSLabFJHService.DataObjects
{
    public class Person : EntityData
    {
        public string Name { get; set; }
        public string ExternalId { get; set; }
    }
}