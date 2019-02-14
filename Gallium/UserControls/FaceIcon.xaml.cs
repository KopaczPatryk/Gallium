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
    public delegate void OnFaceIconAcceptedHandler();
    public delegate void OnFaceIconRejectedHandler();

    public partial class FaceIcon : UserControl
    {
        public event OnFaceIconAcceptedHandler AcceptedListener;
        public event OnFaceIconRejectedHandler RejectedListener;

        private bool IsCurrentlyAccepted = false;
        private DetectedFace Face;

        public FaceIcon(DetectedFace face, bool isAccepted)
        {
            IsCurrentlyAccepted = isAccepted;
            Face = face;

            InitializeComponent();

            Loaded += FaceIcon_Loaded;
        }

        private void FaceIcon_Loaded(object sender, RoutedEventArgs e)
        {
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(DirectoryHelper.GetFacePath(Face.FaceFile), UriKind.Absolute));
            FaceAction.Background = brush;
        }

        private void FaceAction_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsCurrentlyAccepted)
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Assets/icons/nok.png", UriKind.Relative));
                FaceAction.Background = brush;
            }
            else
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Assets/icons/ok.png", UriKind.Relative));
                FaceAction.Background = brush;
            }
        }

        private void FaceAction_MouseLeave(object sender, MouseEventArgs e)
        {
            FaceAction.Background = Brushes.Transparent;
        }

        private void FaceAction_Click(object sender, RoutedEventArgs e)
        {
            if (IsCurrentlyAccepted)
            {
                RejectedListener?.Invoke();
            }
            else
            {
                AcceptedListener?.Invoke();
            }
        }
    }
}
