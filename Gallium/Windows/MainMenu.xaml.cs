using Gallium.Data;
using Gallium.Helpers;
using Gallium.Models;
using Microsoft.ProjectOxford.Face;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using System.Windows.Forms;
using Gallium.Models.FaceApi;

namespace Gallium.Windows
{
    public partial class MainMenu : Window
    {
        private IFaceServiceClient FaceClient;
        private GalliumContext Context;
        BackgroundWorker FaceApiWorker;
        
        public MainMenu()
        {
            Context = new GalliumContext();
            FaceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);
            InitializeComponent();

            InitPersonGroup();
            InitWorkingDirectory();

            SynchroniseData();
            
            FaceApiWorker = new BackgroundWorker();
            FaceApiWorker.DoWork += FaceApiWorker_DoWorkAsync;
            FaceApiWorker.ProgressChanged += FaceApiWorker_ProgressChanged;
            FaceApiWorker.RunWorkerCompleted += FaceApiWorker_RunWorkerCompleted;
            FaceApiWorker.WorkerReportsProgress = true;
            FaceApiWorker.RunWorkerAsync();
        }

        private void FaceApiWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine($"Done analysing photos");

        }

        private void FaceApiWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine($"{e.ProgressPercentage} percent of photos analysed");
        }

        private void FaceApiWorker_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            using (GalliumContext ctx = new GalliumContext())
            {
                List<PhotoDirectories> directories = ctx.Directories.ToList();
                IList<Photo> photos = PhotoHelper.DiscoverPhotosInDirectories(directories);

                AddPhotosToDB(ctx, photos);
                
                photos = ctx.Photos.Where(p => p.HasFacesChecked == false).Include(f => f.DetectedFaces).ToList().Join(photos, a => a.Name, b => b.Name, (a, b) => b).ToList();
                DetectFacesOnPhotosAsync(ctx, photos);
            }
        }

        private void AddPhotosToDB(GalliumContext ctx, IList<Photo> photos)
        {
            foreach (var photo in photos)
            {
                if (!ctx.Photos.Where(p => p.FullName.Equals(photo.FullName)).Any())
                {
                    ctx.Photos.Add(photo);
                }
            }
            ctx.SaveChanges();
        }

        private void DetectFacesOnPhotosAsync(GalliumContext ctx, IList<Photo> photos)
        {
            float startCount = photos.Count;
            while (photos.Any())
            {
                FaceApiWorker.ReportProgress(100 - (int)(photos.Count / startCount * 100));

                ProcessPhoto(ctx, photos.First());
                photos.Remove(photos.First());
            }
        }

        private void ProcessPhoto(GalliumContext ctx, Photo photo)
        {
            Console.WriteLine($"Beginning processing of photo {photo.Name}");
            try
            {
                if (!photo.HasFacesChecked)
                {
                    photo.HasFacesChecked = true;

                    var faces = PhotoHelper.UploadToFaceApiAsync(photo.FullName).Result;

                    foreach (var face in faces)
                    {
                        var faceEntity = new Models.FaceApi.DetectedFace
                        {
                            HumanVerified = false,
                            FaceRectangle = face.FaceRectangle,
                            Photo = photo,
                            FaceId = face.FaceId
                        };
                        PhotoHelper.ExtractFaceFromPhoto(photo, faceEntity);
                        faceEntity.FaceFile = DirectoryHelper.GetFacePath(face.FaceId.ToString());
                        ctx.DetectedFaces.Add(faceEntity);
                    }
                    ctx.SaveChanges();
                }
            }
            catch (System.IO.IOException e) { Thread.Sleep(500); }
            catch (Exception e)
            {
                Thread.Sleep(5000);
            }
        }

        private async void InitPersonGroup()
        {
            var personGroups = await FaceClient.ListLargePersonGroupsAsync();
            if (!personGroups.Where(pg => pg.LargePersonGroupId.Equals(Constants.MainPersonGroupId)).Any())
            {
                await FaceClient.CreateLargePersonGroupAsync(Constants.MainPersonGroupId, "Wszyscy");
            }
        }

        private void InitWorkingDirectory()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.GalleryMainFolder))
            {
                System.Windows.MessageBox.Show("Folder w którym mają być przechowywane pliki pomocnicze nie został jeszcze wybrany. Teraz otworzę okno wyboru folderu głównego.");
                VistaFolderBrowserDialog setupDirectory = new VistaFolderBrowserDialog();
                setupDirectory.Description = "Wybierz lokalizacje pomocniczą.";
                setupDirectory.ShowDialog();
                if (!string.IsNullOrEmpty(setupDirectory.SelectedPath))
                {
                    Properties.Settings.Default.GalleryMainFolder = setupDirectory.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
            Console.WriteLine(Properties.Settings.Default.GalleryMainFolder);
        }

        private async void SynchroniseData()
        { 
            await UpdateLocalDB();
            await UpdateRemoteDB();
        }

        private async Task UpdateLocalDB()
        {
            var peopleInFaceApi = await FaceClient.ListPersonsInLargePersonGroupAsync(Constants.MainPersonGroupId);

            if (peopleInFaceApi.Any())
            {
                foreach (var person in peopleInFaceApi)
                {
                    if (!Context.Person.Where(p => (p.Name + "_" + p.LastName).Equals(person.Name)).Any())
                    {
                        var personEntity = new Models.Person
                        {
                            Name = person.Name.Substring(0, person.Name.IndexOf('_')),
                            LastName = person.Name.Substring(person.Name.IndexOf('_') + 1),
                            RemoteGuid = person.PersonId
                        };
                        Context.Person.Add(personEntity);
                        await Context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task UpdateRemoteDB()
        {
            var localPeople = Context.Person.ToList();
            var peopleInFaceApi = await FaceClient.ListPersonsInLargePersonGroupAsync(Constants.MainPersonGroupId);

            foreach (var person in localPeople)
            {
                if (!peopleInFaceApi.Where(p => p.Name.Equals(person.Name + "_" + person.LastName)).Any())
                {
                    await FaceClient.CreatePersonInLargePersonGroupAsync(Constants.MainPersonGroupId, $"{person.Name}_{person.LastName}");
                }
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void Button_Gallery_Click(object sender, RoutedEventArgs e)
        {
            Gallery gallery = new Gallery();
            gallery.Show();
        }
        
        private void FaceVerification_Click(object sender, RoutedEventArgs e)
        {
            FaceVerificationMenu faceVerificationMenu = new FaceVerificationMenu();
            faceVerificationMenu.ShowDialog();
        }
    }
}
