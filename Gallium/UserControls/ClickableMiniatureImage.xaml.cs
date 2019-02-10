using Gallium.Data;
using Gallium.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public delegate void OnMiniatureLoadedHandler();

    public partial class ClickableMiniatureImage : UserControl, ILoadableImage
    {
        public event OnMiniatureLoadedHandler MiniatureLoaded;

        private Gallery parrentWindow;
        private Photo photo;

        BackgroundWorker backgroundWorker;

        public ClickableMiniatureImage(Gallery gallery, Photo photo)
        {
            InitializeComponent();
            this.parrentWindow = gallery;
            this.photo = photo;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(e.Argument.ToString(), UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            bitmap.Freeze();
            e.Result = bitmap;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MiniatureImage.Source = (BitmapImage)e.Result;
            }
            MiniatureLoaded();
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            parrentWindow.ShowPreview(photo);
        }

        public void LoadImage ()
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(photo.Miniature.MiniatureFullPath);
            }
        }
    }
}
