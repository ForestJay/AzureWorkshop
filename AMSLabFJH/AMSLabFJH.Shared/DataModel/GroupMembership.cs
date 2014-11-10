using System;
using System.Collections.Generic;
using System.Text;

namespace AMSLabFJH.DataModel
{
    class GroupMembership
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string PersonId { get; set; }
        public bool HasViewingPrivilege { get; set; }
    }
}
