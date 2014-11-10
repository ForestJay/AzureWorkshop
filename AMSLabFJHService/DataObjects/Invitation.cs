using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AMSLabFJHService.DataObjects
{
    public class Invitation : EntityData
    {
        public string GroupId { get; set; }
        public bool Accepted { get; set; }
    }
}