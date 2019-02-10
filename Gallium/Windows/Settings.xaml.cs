using Gallium.Data;
using Gallium.Models;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private GalliumContext ctx = new GalliumContext();
        private BindingList<PhotoDirectories> directoriesList;
        public Settings()
        {
            Closed += Settings_Closed;
            InitializeComponent();
            directoriesList = new BindingList<PhotoDirectories>(ctx.Directories.ToList());
            Directories.ItemsSource = directoriesList;
        }

        private void Settings_Closed(object sender, EventArgs e)
        {
            ctx.Dispose();
        }

        private void Button_AddDir_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            folder.ShowDialog();
            if (!String.IsNullOrEmpty(folder.SelectedPath))
            {
                bool alreadyAdded = ctx.Directories.Any(d => d.Path.Equals(folder.SelectedPath));
                if (!alreadyAdded)
                {
                    var path = new PhotoDirectories() { Path = folder.SelectedPath };
                    ctx.Directories.Add(path);
                    directoriesList.Add(path);
                    ctx.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Ścieżka jest już dodana bądź jest nieprawidłowa.");
                }
            }
        }

        private void Button_DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(Directories.SelectedIndex);
            if (Directories.SelectedIndex >= 0 && Directories.SelectedIndex <= directoriesList.Count)
            {
                ctx.Directories.Remove(directoriesList[Directories.SelectedIndex]);
                ctx.SaveChanges();
                directoriesList.RemoveAt(Directories.SelectedIndex);
            }
        }
    }
}
