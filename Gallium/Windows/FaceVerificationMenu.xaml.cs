using Gallium.Data;
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
        private GalliumContext context;
        public FaceVerificationMenu()
        {
            InitializeComponent();
            context = new GalliumContext();
        }

        private void Button_FaceRegister_Click(object sender, RoutedEventArgs e)
        {
            People peopleWindow = new People(context.Person.ToList(), context);
            peopleWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var unknownFace = context.DetectedFaces.Where(f => f.FaceOwner == null).FirstOrDefault();
            if (unknownFace != null)
            {
                FaceRecognitionWindow faceRecognitionWindow = new FaceRecognitionWindow(context, unknownFace);
                faceRecognitionWindow.OnFaceRecognised += FaceRecognitionWindow_OnFaceRecognised;
                faceRecognitionWindow.Show();
            }
        }

        private void FaceRecognitionWindow_OnFaceRecognised(Models.Person person, bool continueValidation)
        {
            var unknownFace = context.DetectedFaces.Where(f => f.FaceOwner == null).FirstOrDefault();

            FaceRecognitionWindow faceRecognitionWindow = new FaceRecognitionWindow(context, unknownFace, continueValidation);
            faceRecognitionWindow.OnFaceRecognised += FaceRecognitionWindow_OnFaceRecognised;
            faceRecognitionWindow.Show();
        }
    }
}
