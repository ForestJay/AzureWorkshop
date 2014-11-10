using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AMSLabFJHService.DataObjects 
{
    public class GroupMembership : EntityData
    {
        public string GroupId { get; set; }
        public string PersonId { get; set; }
        public bool HasViewingPrivilege { get; set; }
    }
}