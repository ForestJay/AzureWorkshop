using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMSLabFJHService.DataObjects
{
    public class GroupStatus
    {
        public string GroupName { get; set; }
        public GroupMemberStatus[] MemberStatuses { get; set; }
    }
}