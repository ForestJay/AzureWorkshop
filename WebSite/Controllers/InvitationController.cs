using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSite.Data;
using Microsoft.AspNet.Identity.Owin;
using WebSite.Models;

namespace WebSite.Controllers
{
    [Authorize]
    public class InvitationController : Controller
    {
        // GET: Invitation
        public async Task<ActionResult> Index(string id)
        {
            var db = HttpContext.GetOwinContext().Get<AmsDb>();
            var q =
                from i in db.Invitations
                where i.Id == id
                join g in db.Groups on i.GroupId equals g.Id
                join o in db.People on g.OwnerId equals o.Id
                select new
                {
                    i.Accepted,
                    GroupName = g.Name,
                    GroupOwnerName = o.Name
                };
            var result = await q.SingleOrDefaultAsync();

            if (result == null)
            {
                return HttpNotFound();
            }

            if (result.Accepted)
            {
                return View("AlreadyAccepted");
            }

            var vm = new InvitationViewModel
            {
                InvitationId = id,
                GroupOwnerName = result.GroupOwnerName,
                GroupName = result.GroupName
            };

            return View(vm);
        }

        public async Task<ActionResult> Accept(string id)
        {
            var db = HttpContext.GetOwinContext().Get<AmsDb>();
            Invitation invitation = await db.Invitations.SingleOrDefaultAsync(i => i.Id == id);

            if (invitation == null)
            {
                return HttpNotFound();
            }

            if (invitation.Accepted)
            {
                return View("AlreadyAccepted");
            }

            var userId = ApplicationUser.GetUserId(User.Identity);
            var gm = new GroupMembership
            {
                GroupId = invitation.GroupId,
                PersonId = userId
            };
            db.GroupMemberships.Add(gm);
            invitation.Accepted = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Accepted", new { id });
        }

        public async Task<ActionResult> Accepted(string id)
        {
            var userId = ApplicationUser.GetUserId(User.Identity);
            var db = HttpContext.GetOwinContext().Get<AmsDb>();
            var q =
                from gm in db.GroupMemberships
                join i in db.Invitations on gm.GroupId equals i.GroupId
                join g in db.Groups on i.GroupId equals g.Id
                where i.Id == id && i.Accepted && gm.PersonId == userId
                select g.Name;
            var result = await q.SingleOrDefaultAsync();

            if (result == null)
            {
                // Either there is no invitation with this id, or the current user does
                // not belong to the group to which this invitation refers.
                return HttpNotFound();
            }

            var vm = new InvitationAcceptedViewModel { GroupName = result };

            return View("Accepted", vm);
        }
    }
}