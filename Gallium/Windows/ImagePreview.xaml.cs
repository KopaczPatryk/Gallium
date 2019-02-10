using Gallium.Data;
using Gallium.UserControls;
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
    public partial class ImagePreview : Window
    {
        public int current { get; set; } = 0;
        public Photo CurrentPhoto { get; private set; }
        public IList<Photo> photos { get; set; }
        private BitmapImage photoFile;
        
        public ImagePreview(IList<Photo> photos, int currentIndex)
        {
            this.photos = photos;
            current = currentIndex;
            CurrentPhoto = photos[current];
            InitializeComponent();
            Loaded += ImagePreview_Loaded;
        }

        private void ImagePreview_Loaded(object sender, RoutedEventArgs e)
        {
            updateDisplay();
        }

        public void updateDisplay()
        {
            photoFile = new BitmapImage();
            photoFile.BeginInit();
            photoFile.UriSource = new Uri(CurrentPhoto.FullName);
            photoFile.EndInit();

            TargetImage.Source = photoFile;
            TargetImage.SizeChanged += TargetImage_SizeChanged;
        }

        private void TargetImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Faces.Children.Clear();
            if (CurrentPhoto.DetectedFaces != null && CurrentPhoto.DetectedFaces.Any())
            {
                foreach (var face in CurrentPhoto.DetectedFaces)
                {
                    var rect = new FaceDetails(face, TargetImage.RenderSize, photoFile.PixelWidth, photoFile.PixelHeight);

                    Faces.Children.Add(rect);
                }
            }
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement sendr = (UIElement)sender;
            throw new NotImplementedException();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (current > 0)
            {
                current--;
                CurrentPhoto = photos[current];
                updateDisplay();
                Console.WriteLine(photos.Count);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (current < photos.Count - 1)
            {
                current++;
                CurrentPhoto = photos[current];
                updateDisplay();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
