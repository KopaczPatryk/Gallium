using Gallium.Models.FaceApi;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class FaceDetails : UserControl
    {
        private const double HiddenRectOpacity = 0.25;
        private const double VisibleRectOpacity = 1;
        private DetectedFace face;

        public FaceDetails(DetectedFace face, System.Windows.Size imageSize, int imgPixelWidth, int imgPixelHeight)
        {
            this.face = face;
            InitializeComponent();
            
            PersonNames.Text = $"{face.FaceOwner.Name} {face.FaceOwner.LastName}";
            if (face.FaceOwner.DateOfBirth.HasValue)
            {
                PersonAge.Text = $"{(int)((DateTime.Now - face.FaceOwner.DateOfBirth.Value).TotalDays) / 365} lat";
            }

            sta.MouseEnter += FaceRectangle_MouseEnter;
            sta.MouseLeave += FaceRectangle_MouseLeave;

            var width = imageSize.Width;
            var height = imageSize.Height;

            var xRatio = width / imgPixelWidth;
            var yRatio = height / imgPixelHeight;

            var posX = face.FaceRectangle.Left;
            var posY = face.FaceRectangle.Top;

            var renderXpos = xRatio * posX;
            var renderYpos = yRatio * posY;

            Margin = new Thickness(renderXpos, renderYpos, 0, 0);
            FaceRectangle.Width = face.FaceRectangle.Width * xRatio;
            FaceRectangle.Height = face.FaceRectangle.Height * yRatio;
        }

        private void FaceRectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            setVisiblity(true);
        }
        private async void FaceRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            await Task.Delay(1000).ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(new Action<bool>(setVisiblity), false);
            });
        }

        private void setVisiblity(bool visible)
        {
            switch (visible)
            {
                case true:
                    FaceProperties.Visibility = Visibility.Visible;
                    FaceRectangle.Opacity = VisibleRectOpacity;
                    break;
                case false:
                    FaceProperties.Visibility = Visibility.Hidden;
                    FaceRectangle.Opacity = HiddenRectOpacity;
                    break;
            }
        }

        private void ChangeFaceOwner_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
