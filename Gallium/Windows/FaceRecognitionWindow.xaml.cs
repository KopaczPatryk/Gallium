using Gallium.Data;
using Gallium.Models;
using Gallium.Models.FaceApi;
using Microsoft.ProjectOxford.Face;
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
    public delegate void OnRecognitionPostponed(bool continueValidation);
    public delegate void OnFaceCorrupted(bool continueValidation);

    public partial class FaceRecognitionWindow : Window
    {
        public event OnFaceRecognised FaceRecognised;
        public event OnRecognitionPostponed RecognitionPostponed;
        public event OnFaceCorrupted FaceCorrupted;

        private Person RecognizedPerson;
        private GalliumContext Context;
        private DetectedFace UnrecognisedFace;
        public FaceRecognitionWindow(GalliumContext context, DetectedFace unrecognisedFace, bool continueValidationOnEnd = false)
        {
            Context = context;
            UnrecognisedFace = unrecognisedFace;

            InitializeComponent();
            ContinueValidating.IsChecked = continueValidationOnEnd;
            
            FaceImage.Source = new BitmapImage(new Uri(unrecognisedFace.FaceFile));

            UsernameList.ItemsSource = Context.Person.ToList();
            UsernameList.SelectionChanged += UsernameList_SelectionChanged;
        }

        private void UsernameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecognizedPerson = (Person)UsernameList.SelectedItem;
        }

        private void NewPerson_Click(object sender, RoutedEventArgs e)
        {
            PersonCreationWindow personCreationWindow = new PersonCreationWindow();
            personCreationWindow.OnPersonCreatedHandler += PersonCreationWindow_OnPersonCreatedHandlerAsync;
            personCreationWindow.ShowDialog();
        }

        private async void PersonCreationWindow_OnPersonCreatedHandlerAsync(Person person)
        {
            var FaceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);

            var creationResult = await FaceClient.CreatePersonInLargePersonGroupAsync(Constants.MainPersonGroupId, $"{person.Name}_{person.LastName}");
            Guid personId = creationResult.PersonId;
            person.RemoteGuid = personId;

            Context.Person.Add(person);
            Context.SaveChanges();

            UsernameList.ItemsSource = Context.Person.ToList();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (RecognizedPerson != null)
            {
                if (ContinueValidating.IsChecked.HasValue)
                {
                    FaceRecognised?.Invoke(RecognizedPerson, ContinueValidating.IsChecked.Value);
                }
                else
                {
                    FaceRecognised?.Invoke(RecognizedPerson, false);

                }
                ClearSubscribers();
                FaceRecognised = null;
                Close();
            }
        }
        
        private void Postpone_Click(object sender, RoutedEventArgs e)
        {
            RecognitionPostponed?.Invoke(ContinueValidating.IsChecked.Value);
            ClearSubscribers();
            Close();
        }

        private void NotAFace_Click(object sender, RoutedEventArgs e)
        {
            FaceCorrupted?.Invoke(ContinueValidating.IsChecked.Value);
            ClearSubscribers();
            Close();
        }

        private void ClearSubscribers()
        {
            FaceRecognised = null;
            RecognitionPostponed = null;
            FaceCorrupted = null;
        }
    }
}