using Gallium.Data;
using Microsoft.ProjectOxford.Face;
using Ookii.Dialogs.Wpf;
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
    public partial class MainMenu : Window
    {
        private IFaceServiceClient FaceClient;
        private GalliumContext Context;

        public MainMenu()
        {
            Context = new GalliumContext();
            FaceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);
            InitializeComponent();

            InitPersonGroup();
            InitWorkingDirectory();

            SynchroniseData();
        }

        private async void InitPersonGroup()
        {
            var personGroups = await FaceClient.ListLargePersonGroupsAsync();
            if (!personGroups.Where(pg => pg.LargePersonGroupId.Equals(Constants.MainPersonGroupId)).Any())
            {
                await FaceClient.CreateLargePersonGroupAsync(Constants.MainPersonGroupId, "Wszyscy");
            }
        }

        private void InitWorkingDirectory()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.GalleryMainFolder))
            {
                MessageBox.Show("Folder w którym mają być przechowywane pliki pomocnicze nie został jeszcze wybrany. Teraz otworzę okno wyboru folderu głównego.");
                VistaFolderBrowserDialog setupDirectory = new VistaFolderBrowserDialog();
                setupDirectory.Description = "Wybierz lokalizacje pomocniczą.";
                setupDirectory.ShowDialog();
                if (!string.IsNullOrEmpty(setupDirectory.SelectedPath))
                {
                    Properties.Settings.Default.GalleryMainFolder = setupDirectory.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
            Console.WriteLine(Properties.Settings.Default.GalleryMainFolder);
        }

        private async void SynchroniseData()
        { 
            await UpdateLocalDB();
            await UpdateRemoteDB();
        }

        private async Task UpdateLocalDB()
        {
            var peopleInFaceApi = await FaceClient.ListPersonsInLargePersonGroupAsync(Constants.MainPersonGroupId);

            if (peopleInFaceApi.Any())
            {
                foreach (var person in peopleInFaceApi)
                {
                    if (!Context.Person.Where(p => ($"{p.Name}_{p.LastName}").Equals(person.Name)).Any())
                    {
                        var personEntity = new Models.Person
                        {
                            Name = person.Name.Substring(0, person.Name.IndexOf('_')),
                            LastName = person.Name.Substring(person.Name.IndexOf('_') + 1),
                            RemoteGuid = person.PersonId
                        };
                        Context.Person.Add(personEntity);
                        await Context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task UpdateRemoteDB()
        {
            var localPeople = Context.Person.ToList();
            var peopleInFaceApi = await FaceClient.ListPersonsInLargePersonGroupAsync(Constants.MainPersonGroupId);

            foreach (var person in localPeople)
            {
                if (!peopleInFaceApi.Where(p => p.Name.Equals($"{person.Name}_{person.LastName}")).Any())
                {
                    await FaceClient.CreatePersonInLargePersonGroupAsync(Constants.MainPersonGroupId, $"{person.Name}_{person.LastName}");
                }
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void Button_Gallery_Click(object sender, RoutedEventArgs e)
        {
            Gallery gallery = new Gallery();
            gallery.Show();
        }
        
        private void FaceVerification_Click(object sender, RoutedEventArgs e)
        {
            FaceVerificationMenu faceVerificationMenu = new FaceVerificationMenu();
            faceVerificationMenu.ShowDialog();
        }
    }
}
