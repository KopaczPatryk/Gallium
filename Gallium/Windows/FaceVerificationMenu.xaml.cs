using Gallium.Data;
using Gallium.Models.FaceApi;
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
    /// Interaction logic for FaceVerificationMenu.xaml
    /// </summary>
    public partial class FaceVerificationMenu : Window
    {
        private GalliumContext Context;

        private DetectedFace CurrentUnknownFace;
        public FaceVerificationMenu()
        {
            InitializeComponent();
            Context = new GalliumContext();
        }

        private void PersonProfiles_Click(object sender, RoutedEventArgs e)
        {
            PeopleProfilesWindow peopleWindow = new PeopleProfilesWindow(Context);
            peopleWindow.ShowDialog();
        }

        private void LaunchFaceRecognitionTool_Click(object sender, RoutedEventArgs e)
        {
            ShowRecognitionWindow();
        }

        private void ShowRecognitionWindow(bool continueValidation = false)
        {
            CurrentUnknownFace = Context.DetectedFaces.Where(f => f.FaceOwner == null && f.Postponed == false).FirstOrDefault();

            FaceRecognitionWindow faceRecognitionWindow = new FaceRecognitionWindow(Context, CurrentUnknownFace, continueValidation);
            faceRecognitionWindow.FaceRecognised += FaceRecognitionWindow_OnFaceRecognised;
            faceRecognitionWindow.RecognitionPostponed += FaceRecognitionWindow_PostponeListener;
            faceRecognitionWindow.FaceCorrupted += FaceRecognitionWindow_CorruptedListener;
            faceRecognitionWindow.Show();
        }

        private void FaceRecognitionWindow_OnFaceRecognised(Models.Person person, bool continueValidation)
        {
            CurrentUnknownFace.FaceOwner = person;
            Context.SaveChanges();
            if (continueValidation)
            {
                ShowRecognitionWindow(continueValidation);
            }
        }
        
        private void FaceRecognitionWindow_CorruptedListener(bool continueValidation)
        {
            CurrentUnknownFace.IsValidFace = false;
            Context.SaveChanges();
            if (continueValidation)
            {
                ShowRecognitionWindow(continueValidation);
            }
        }

        private void FaceRecognitionWindow_PostponeListener(bool continueValidation)
        {
            CurrentUnknownFace.Postponed = true;
            Context.SaveChanges();
            if (continueValidation)
            {
                ShowRecognitionWindow(continueValidation);
            }
        }
    }
}
