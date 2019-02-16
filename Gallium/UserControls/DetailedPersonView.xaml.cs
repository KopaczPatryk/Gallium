using Gallium.Data;
using Gallium.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gallium.UserControls
{
    /// <summary>
    /// Interaction logic for DetailedPersonView.xaml
    /// </summary>
    public partial class DetailedPersonView : UserControl
    {
        public DetailedPersonView(Person person, GalliumContext context)
        {
            this.DataContext = person;
            InitializeComponent();
            GroupBox.Header = $"{person.Name} {person.LastName}";

            //FullName.Content = $"{person.Name} {person.LastName}";
            DateOfBirth.Content = person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
            var re = context.DetectedFaces.Where(f => f.FaceOwner.Id == person.Id).ToList();
            foreach (var face in re)
            {
                /*
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();

                image.Source = new BitmapImage(new Uri(face.FaceFile));*/
                VerifiedFaces.Children.Add(new FaceIcon(face, face.HumanVerified));
            }
        }
    }
}
