using Gallium.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gallium.UserControls
{
    public delegate void OnFaceIconAcceptedHandler(FaceIcon ths);
    public delegate void OnFaceIconRejectedHandler(FaceIcon ths);

    public partial class FaceIcon : UserControl
    {
        public event OnFaceIconAcceptedHandler AcceptedListener;
        public event OnFaceIconRejectedHandler RejectedListener;

        //private bool IsCurrentlyAccepted = false;
        private DetectedFace Face;

        public FaceIcon(DetectedFace face)
        {
            Face = face;

            InitializeComponent();

            Loaded += FaceIcon_Loaded;
        }

        private void FaceIcon_Loaded(object sender, RoutedEventArgs e)
        {
            FaceImage.Source = new BitmapImage(new Uri(DirectoryHelper.GetFacePath(Face.FaceId.ToString()), UriKind.Absolute));
            
            ManageBackground();
        }
                
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ManageBackground();
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            //IsCurrentlyAccepted = !IsCurrentlyAccepted;
            if (Face.HumanVerified)
            {
                RejectedListener?.Invoke(this);
            }
            else
            {
                AcceptedListener?.Invoke(this);
            }
            ManageBackground();
        }

        private void ManageBackground()
        {
            var brush = new ImageBrush();

            if (Face.HumanVerified)
            {
                brush.ImageSource = new BitmapImage(new Uri("Assets/icons/nok.png", UriKind.Relative));
                Action.Background = brush;
            }
            else
            {
                brush.ImageSource = new BitmapImage(new Uri("Assets/icons/ok.png", UriKind.Relative));
                Action.Background = brush;
            }
        }
    }
}