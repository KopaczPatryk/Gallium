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
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();

            InitPersonGroup();
            InitWorkingDirectory();
        }

        private async void InitPersonGroup()
        {
            var FaceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);

            var personGroups = FaceClient.ListLargePersonGroupsAsync().Result;
            if (!personGroups.Where(pg => pg.LargePersonGroupId.Equals(Constants.MainPersonGroupId)).Any())
            {
                await FaceClient.CreatePersonGroupAsync(Constants.MainPersonGroupId, "Wszyscy");
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
