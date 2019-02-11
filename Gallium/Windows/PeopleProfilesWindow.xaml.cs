using Gallium.Data;
using Gallium.Models;
using Gallium.UserControls;
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gallium.Windows
{
    /// <summary>
    /// Interaction logic for People.xaml
    /// </summary>
    public partial class PeopleProfilesWindow : Window
    {
        GalliumContext Context;

        public PeopleProfilesWindow(GalliumContext context)
        {
            Context = context;
            InitializeComponent();
            PopulateView();
        }

        private void PopulateView()
        {
            foreach (var person in Context.Person.ToList())
            {
                PeopleList.Children.Add(new DetailedPersonView(person, Context));
            }
        }

        private void CreatePersonButton_Click(object sender, RoutedEventArgs e)
        {
            var personCreationWindow = new PersonCreationWindow();
            personCreationWindow.OnPersonCreatedHandler += PersonCreationWindow_OnPersonCreatedHandler;
            personCreationWindow.Show();
        }

        private async void PersonCreationWindow_OnPersonCreatedHandler(Person person)
        {
            var FaceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);

            var result = await FaceClient.CreatePersonInLargePersonGroupAsync(Constants.MainPersonGroupId, $"{person.Name}_{person.LastName}");
            person.RemoteGuid = result.PersonId;

            Context.Person.Add(person);
            await Context.SaveChangesAsync();
            PopulateView();
        }
    }
}
