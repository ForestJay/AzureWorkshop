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
using System.Net.Http;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AMSLabFJH
{
    public sealed partial class GroupView : UserControl
    {
        private string groupId;

        public GroupView()
        {
            this.InitializeComponent();
        }

        public async void Load(string groupId)
        {
            this.groupId = groupId;
            MobileServiceInvalidOperationException exception = null;
            List<GroupMemberViewModel> items = null;

            try
            {
                GroupStatus result = await App.MobileService.InvokeApiAsync<GroupStatus>(
                  "GroupStatus",
                  HttpMethod.Get,
                 new Dictionary<string, string> { { "id", groupId } });

                GroupNameTextBlock.Text = result.GroupName;

                items = result.MemberStatuses
                    .Select(
                        ms =>
                            new GroupMemberViewModel
                            {
                                PersonName = ms.PersonName,
                                CheckInDetails = ms.CheckInLocation == null
                                    ? "Not checked in recently"
                                    : string.Format("Checked in on {0} at {1}", ms.CheckInTime, ms.CheckInLocation)

                            })
                    .ToList();
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
            var invitation = new Invitation { GroupId = groupId };
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
