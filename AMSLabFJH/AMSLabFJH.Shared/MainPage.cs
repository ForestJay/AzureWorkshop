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
                // This code refreshes the entries     // in the list view by querying the Persons table.
                // The query excludes completed Persons
                person = await peopleTable.LookupAsync("1");
                userNameTextBox.Text = person.Name;

                items = await groupsTable.Where(g => g.OwnerId ==
                  person.Id).ToCollectionAsync();
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

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }

        private async void ChangeName_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
