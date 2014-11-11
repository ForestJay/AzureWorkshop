using System;
using System.Collections.Generic;
using System.Text;

namespace AMSLabFJH.DataModel
{
    class GroupStatus
    {
        public string GroupName { get; set; }
        public GroupMemberStatus[] MemberStatuses { get; set; }
    }
}
