using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AMSLabFJH.DataModel;
using Windows.Security.Credentials;
using System.Linq;
using System.Net;
using Windows.Devices.Geolocation;

namespace AMSLabFJH
{
    sealed partial class MainPage: Page
    {
        private MobileServiceCollection<Group, Group> items;
        private IMobileServiceTable<Group> groupsTable = App.MobileService.GetTable<Group>();
        private IMobileServiceTable<Person> peopleTable = App.MobileService.GetTable<Person>();
        private Person person = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task RefreshData()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                var vault = new PasswordVault();
                PasswordCredential credentials = null;
                try
                {
                    // Try to get an existing credential from the vault.
                    credentials = vault.RetrieveAll().FirstOrDefault();
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }

                if (credentials != null)
                {
                    var user = new MobileServiceUser(credentials.UserName);
                    credentials.RetrievePassword();
                    user.MobileServiceAuthenticationToken = credentials.Password;

                    // Set the user from the stored credentials.
                    App.MobileService.CurrentUser = user;
                    try
                    {
                        await groupsTable.Take(1).ToListAsync();
                    }
                    catch (MobileServiceInvalidOperationException x)
                    {
                        if (x.Response == null || x.Response.StatusCode != HttpStatusCode.Unauthorized)
                        {
                            // Don't know what this is, so pass it on.
                            throw;
                        }

                        App.MobileService.CurrentUser = null;
                        vault.Remove(credentials);
                        credentials = null;
                    }
                }

                if (credentials == null)
                {
                    MobileServiceUser user =
                await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    credentials = new PasswordCredential(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory.ToString(), user.UserId, user.MobileServiceAuthenticationToken);

                    vault.Add(credentials);

                }
                else
                {
                    var user = new MobileServiceUser(credentials.UserName);
                    credentials.RetrievePassword();
                    user.MobileServiceAuthenticationToken = credentials.Password;

                    // Set the user from the stored credentials.
                    App.MobileService.CurrentUser = user;
                }

                // This code refreshes the entries     // in the list view by querying the Persons table.
                // The query excludes completed Persons
                person = (await peopleTable.ReadAsync()).SingleOrDefault();

                if (person == null)
                {
                    var mb = new MessageDialog(
                        "This is the first time you've logged in. Please type in your name and then tap Change to set up your account.");
                    await mb.ShowAsync();
                    this.changeNameButton.IsEnabled = true;
                    return;
                }

                userNameTextBox.Text = person.Name;

                items = await groupsTable.ToCollectionAsync();
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
                ListItems.ItemsSource = items;
                this.changeNameButton.IsEnabled = true;
            }
        }

        private async void ButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            var g = new Group { Name = groupNameTextBox.Text };
            await groupsTable.InsertAsync(g);
            await RefreshData();
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }

        private async void ChangeName_Click(object sender, RoutedEventArgs e)
        {
            if (person == null)
            {
                person = new Person { Name = userNameTextBox.Text };
                await peopleTable.InsertAsync(person);
                return;
            }
            person.Name = userNameTextBox.Text;
            await peopleTable.UpdateAsync(person);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Dispatcher.RunIdleAsync(async args => await RefreshData());
        }

        public void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var group = ((ListView)sender).SelectedItem as Group;
            if (group != null)
            {
                ((Frame)Window.Current.Content).Navigate(typeof(GroupPage), group.Id);
            }
        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            var vault = new PasswordVault();
            PasswordCredential credentials = null;
            do
            {
                try
                {
                    // Try to get an existing credential from the vault.
                    credentials = vault.RetrieveAll().FirstOrDefault();
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }
                if (credentials != null)
                {
                    vault.Remove(credentials);
                }
            } while (credentials != null);

            App.MobileService.CurrentUser = null;
        }

        private async void ButtonCheckIn_Click(object sender, RoutedEventArgs e)
        {
            Exception error = null;
            try
            {
                var loc = new Geolocator();
                Geoposition pos = await loc.GetGeopositionAsync();
                BasicGeoposition pp = pos.Coordinate.Point.Position;
                string positionText =
                  string.Format("{0}, {1}", pp.Latitude, pp.Longitude);
                var c = new CheckIn { Location = positionText };
                await App.MobileService.GetTable<CheckIn>().InsertAsync(c);

            }
            catch (Exception x)
            {
                error = x;
            }

            if (error != null)
            {
                var md = new MessageDialog(error.Message, "Error");
                await md.ShowAsync();
            }
        }
    }
}
