using Gallium.Data;
using Gallium.Models;
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
using System.Windows.Shapes;

namespace Gallium.Windows
{
    public delegate void OnFaceRecognised(Person person, bool continueValidation);

    public partial class FaceRecognitionWindow : Window
    {
        public event OnFaceRecognised OnFaceRecognised;

        private Person recognizedPerson;
        private GalliumContext ctx;
        private DetectedFace face;
        private Photo photoContainingThisFace;
        public FaceRecognitionWindow(GalliumContext context, DetectedFace face, bool continueValidationOnEnd = false)
        {
            ctx = context;
            this.face = face;
            photoContainingThisFace = face.Photo;
            InitializeComponent();
            ContinueValidating.IsChecked = continueValidationOnEnd;
            
            FaceImage.Source = new BitmapImage(new Uri(face.FaceFile));

            UsernameList.ItemsSource = ctx.Person.ToList();
            UsernameList.SelectionChanged += UsernameList_SelectionChanged;
        }

        private void UsernameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recognizedPerson = (Person)UsernameList.SelectedItem;
        }

        private void NewPerson_Click(object sender, RoutedEventArgs e)
        {
            PersonCreationWindow personCreationWindow = new PersonCreationWindow();
            personCreationWindow.OnPersonCreatedHandler += PersonCreationWindow_OnPersonCreatedHandler;
            personCreationWindow.ShowDialog();
        }

        private void PersonCreationWindow_OnPersonCreatedHandler(Person person)
        {
            ctx.Person.Add(person);
            ctx.SaveChanges();
            UsernameList.ItemsSource = ctx.Person.ToList();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            
            if (recognizedPerson != null)
            {
                face.FaceOwner = recognizedPerson;
                if (ContinueValidating.IsChecked.HasValue)
                {
                    if (OnFaceRecognised != null)
                    {
                        OnFaceRecognised(recognizedPerson, ContinueValidating.IsChecked.Value);
                    }
                }
                else
                {
                    if (OnFaceRecognised != null)
                    {
                        OnFaceRecognised(recognizedPerson, false);
                    }
                }
                ctx.SaveChanges();
                this.Close();
            }
        }
    }
}