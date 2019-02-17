using Gallium.Data;
using Gallium.Models;
using Gallium.Models.FaceApi;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gallium.UserControls
{
    public partial class DetailedPersonView : UserControl
    {
        private GalliumContext Context;
        private Person Person;
        public DetailedPersonView(Person person, GalliumContext context)
        {
            Context = context;
            Person = person;

            this.DataContext = person;
            InitializeComponent();
            GroupBox.Header = $"{person.Name} {person.LastName}";
            
            DateOfBirth.Content = person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
            
            PopulateOtherFaces();
            PopulateVerifiedFaces();
        }

        private void FaceIcon_OnDragStarted(FaceIcon ths)
        {
            PopulateOtherFaces();
            PopulateVerifiedFaces();
        }

        private void PopulateVerifiedFaces()
        {
            var humanVerifiedFaces = Context.DetectedFaces.Where(f => f.FaceOwner.Id == Person.Id && f.HumanVerified == true).ToList();
            foreach (var face in humanVerifiedFaces)
            {
                var faceIcon = new FaceIcon(face);
                faceIcon.OnDragStarted += FaceIcon_OnDragStarted;
                VerifiedFaces.Children.Clear();
                VerifiedFaces.Children.Add(faceIcon);
            }
        }

        private void PopulateOtherFaces()
        {
            var otherFaces = Context.DetectedFaces.Where(f => f.FaceOwner.Id == Person.Id && f.HumanVerified == false).ToList();
            foreach (var face in otherFaces)
            {
                var faceIcon = new FaceIcon(face);
                faceIcon.OnDragStarted += FaceIcon_OnDragStarted;
                OtherFaces.Children.Clear();
                OtherFaces.Children.Add(faceIcon);
            }
        }


        private void OtherFaces_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            var draggedFace = (DetectedFace)e.Data.GetData("Face");
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
            OtherFaces.Children.Add(new FaceIcon(draggedFace));
        }

        private void VerifiedFaces_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            var draggedFace = (DetectedFace)e.Data.GetData("Face");
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
            VerifiedFaces.Children.Add(new FaceIcon(draggedFace));
        }

        private void VerifiedFaces_DragOver(object sender, DragEventArgs e)
        {
            OtherFaces.Background = Brushes.Green;
            VerifiedFaces.Background = Brushes.Green;
        }
    }
}
