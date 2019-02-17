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
    public partial class DetailedPersonView : UserControl
    {
        public DetailedPersonView(Person person, GalliumContext context)
        {
            this.DataContext = person;
            InitializeComponent();
            GroupBox.Header = $"{person.Name} {person.LastName}";
            
            DateOfBirth.Content = person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
            var humanVerifiedFaces = context.DetectedFaces.Where(f => f.FaceOwner.Id == person.Id && f.HumanVerified == true).ToList();
            foreach (var face in humanVerifiedFaces)
            {
                VerifiedFaces.Children.Add(new FaceIcon(face));
            }

            var otherFaces = context.DetectedFaces.Where(f => f.FaceOwner.Id == person.Id && f.HumanVerified == false).ToList();
            foreach (var face in otherFaces)
            {
                VerifiedFaces.Children.Add(new FaceIcon(face));
            }
        }
    }
}
