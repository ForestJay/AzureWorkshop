using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;
using AMSLabFJHService.DataObjects;
using AMSLabFJHService.Models;
using AMSLabFJHService.Authentication;
using System.Data.Entity;
using System.Web;
using System.Threading.Tasks;

namespace AMSLabFJHService.Controllers
{
    public class GroupStatusController : ApiController
    {
        
        public ApiServices Services { get; set; }

        [AuthorizeLevel(AuthorizationLevel.User)]

        // GET api/GroupStatus
        public async Task<GroupStatus> Get(string id)
        {
            using (var db = new AMSLabFJHContext())
            {
                string externalId = await UserInformation.ExternalIdFromUser(User);
                var groupNameQuery =
                    from p in db.People
                    join g in db.Groups on p.Id equals g.OwnerId
                    where p.ExternalId == externalId &&
                    g.Id == id
                    select g.Name;
                string groupName = await groupNameQuery.SingleOrDefaultAsync();

                if (groupName == null)
                {
                    throw new HttpException(404, "Not found");
                }

                var statusQuery =
                    from g in db.Groups
                    where g.Id == id
                    join gm in db.GroupMemberships on g.Id equals gm.GroupId
                    join p in db.People on gm.PersonId equals p.Id
                    let lastCheckin = db.CheckIns
                        .Where(c => c.PersonId == p.Id)
                        .OrderByDescending(c => c.CheckInTime)
                        .FirstOrDefault()
                    select new
                    {
                        p.Name,
                        lastCheckin.Location,
                        lastCheckin.CheckInTime
                    };

                GroupMemberStatus[] statuses =
                (await statusQuery.ToListAsync())
                   .Select(
                      i => new GroupMemberStatus
                      {
                         PersonName = i.Name,
                         CheckInTime = i.CheckInTime,
                         CheckInLocation = i.Location
                     })
                 .ToArray();

                return new GroupStatus
                {
                    GroupName = groupName,
                    MemberStatuses = statuses
                };
            }
        }

    }
}
