using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using AMSLabFJH.DataModel;
using Windows.UI.Popups;
//  I think the following is needed (and more) so that Invitation will be recognized!
//using AMSLabFJHService.DataObjects;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Email;
#endif

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AMSLabFJH
{
    public sealed partial class GroupView : UserControl
    {
        private const string WebSiteBase = "http://AMSLabFJH.azurewebsites.net/";
        private Group group;
        private IMobileServiceTable<Person> peopleTable = App.MobileService.GetTable<Person>();
        private IMobileServiceTable<Group> groupsTable = App.MobileService.GetTable<Group>();
        private IMobileServiceTable<CheckIn> checkInsTable = App.MobileService.GetTable<CheckIn>();
        private IMobileServiceTable<GroupMembership> groupMembershipTable = App.MobileService.GetTable<GroupMembership>();
        
        public GroupView()
        {
            this.InitializeComponent();
        }

        public async void Load(string groupId)
        {
            MobileServiceInvalidOperationException exception = null;
            var items = new List<GroupMemberViewModel>();
            try
            {
                group = await groupsTable.LookupAsync(groupId);
                GroupNameTextBlock.Text = group.Name;

                List<GroupMembership> groupMembers =
                    await groupMembershipTable.Where(gm => gm.GroupId == groupId).ToListAsync();
                foreach (GroupMembership member in groupMembers)
                {
                    Person person = await peopleTable.LookupAsync(member.PersonId);

                    var checkInQuery = checkInsTable
                        .Where(ci => ci.PersonId == person.Id)
                        .OrderByDescending(ci => ci.CheckInTime)
                        .Take(1);
                    CheckIn lastCheckIn = (await checkInQuery.ToEnumerableAsync()).SingleOrDefault();


                    var item = new GroupMemberViewModel
                    {
                        PersonName = person.Name,
                        CheckInDetails = lastCheckIn == null
                            ? "Not checked in recently"
                            : string.Format("Checked in on {0} at {1}", lastCheckIn.CheckInTime, lastCheckIn.Location)
                    };
                    items.Add(item);
                }

            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                listItems.ItemsSource = items;
            }
        }

        private async void ButtonInvite_OnClick(object sender, RoutedEventArgs e)
        {
            var invitation = new Invitation { GroupId = group.Id };
            await App.MobileService.GetTable<Invitation>().InsertAsync(invitation);
            string url = string.Format("{0}Invitation/{1}", WebSiteBase, invitation.Id);
            const string messageSubject = "Please join my group";
            string messageBody = string.Format(
          "I'd like you to join my group, to help us stay in touch during emergencies. " +
        "Please click on this link to join: {0}", url);

#if WINDOWS_PHONE_APP
    var em = new EmailMessage
    {
      Subject = messageSubject,
      Body = messageBody
    };
    await EmailManager.ShowComposeNewEmailAsync(em);
#else
            var mailto =
              new Uri(string.Format("mailto:?subject={0}&body={1}",
                messageSubject, messageBody));
            await Windows.System.Launcher.LaunchUriAsync(mailto);
#endif
        }
    }
}
