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
        private MobileServiceCollection<Person, Person> items;
        private IMobileServiceTable<Person> todoTable = App.MobileService.GetTable<Person>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task InsertPerson(Person Person)
        {
            // This code inserts a new Person into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(Person);
            items.Add(Person);
        }

        private async Task RefreshPersons()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the Persons table.
                // The query excludes completed Persons
                items = await todoTable
                    .ToCollectionAsync();
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
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshPersons();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var Person = new Person { Name = TextInput.Text };
            await InsertPerson(Person);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await RefreshPersons();
        }
    }
}
