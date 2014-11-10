using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AMSLabFJHService.DataObjects
{
    public class Group : EntityData
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
    }
}