﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Gallium.Windows;
using Gallium.Data;
using Gallium.UserControls;
using Gallium.Core;
using Gallium.Helpers;
using System.Threading.Tasks;
using Gallium.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Gallium.Models.FaceApi;
using System.Data.Entity;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Gallium
{
    public partial class Gallery : Window
    {
        private const string MiniaturesFolder = @"\miniatures\";
        GalliumContext ctx;
        public ImagePreview preview;

        List<Photo> photos = new List<Photo>();
        List<ILoadableImage> miniatures = new List<ILoadableImage>();

        List<int> miniatureLoadQueue = new List<int>();
        List<int> loadedMiniatureIds = new List<int>();

        MiniatureGenerator miniatureGenerator;
        IFaceServiceClient faceClient;
        
        public Gallery()
        {
            faceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);
            miniatureGenerator = new MiniatureGenerator();
            InitializeComponent();
            ScrollViewMiniatures.ScrollChanged += ScrollViewMiniatures_ScrollChanged;
            Loaded += Gallery_Loaded;
        }

        private async void Gallery_Loaded(object sender, RoutedEventArgs e)
        {
            using (ctx = new GalliumContext())
            {
                var dirs = ctx.Directories.ToList();
                var discoveredPhotos = DiscoverPhotosInDirectories(dirs);
                
                //ewidencjonowanie
                foreach (var photo in discoveredPhotos)
                {
                    if (!ctx.Photos.Where(p => p.FullName.Equals(photo.FullName)).Any())
                    {
                        ctx.Photos.Add(photo);
                    }
                }
                //pobranie tylko tych co są w folderze
                photos = await ctx.Photos.Include(faceClient => faceClient.DetectedFaces.Select(df => df.FaceOwner)).Include(m => m.Miniature).ToListAsync();
                photos = photos.Intersect(discoveredPhotos, new PhotoEqualityComparer()).ToList();
                await ctx.SaveChangesAsync();

                //zapis twarzy do plików
                /*foreach (var photo in photos)
                {
                    if (!photo.DetectedFaces.Any() && photo.HasFaces == true)
                    {
                        DetectedFace detectedFace = new DetectedFace();
                        var r = await UploadToFaceApiAsync(photo.FullName);
                        foreach (var face in r)
                        {
                            detectedFace = new DetectedFace();
                            detectedFace.FaceId = face.FaceId;
                            detectedFace.FaceRectangle = face.FaceRectangle;
                            detectedFace.HumanVerified = false;
                            photo.DetectedFaces.Add(detectedFace);
                            ExtractFaceFromPhoto(photo, detectedFace);
                        }
                        await ctx.SaveChangesAsync();
                    }
                }*/
                List<Photo> faulty = new List<Photo>();
                int bez = 0;
                int z = 0;

                foreach (var photo in photos)
                {
                    if (photo.FullName.Contains("IMG_20180310_165252.jpg"))
                    {
                        Console.WriteLine();
                    }
                    if (photo.Miniature == null)
                    {
                        bez++;
                        try
                        {
                            photo.Miniature = GetMiniatureForPhoto(photo);
                        }
                        catch (ArgumentException)
                        {
                            faulty.Add(photo);
                            Console.WriteLine($"Skipping file {photo.FullName}");
                        }
                    }
                    else
                    {
                        z++;     
                    }
                }
                foreach (var faultyone in faulty)
                {
                    photos.Remove(faultyone);
                }

                foreach (var photo in photos)
                {
                    if (photo.Miniature.MiniatureFileName.Contains("cipeczki"))
                    {
                        Console.WriteLine("dd");
                    }
                    if (photo.Miniature != null)
                    {
                        AddMiniatureToGrid(photo);
                    }
                    else
                    {
                        throw new Exception("Miniature is missing");
                    }
                }
                foreach (ILoadableImage miniature in miniatures)
                {
                    miniature.MiniatureLoaded += Miniature_MiniatureLoaded;
                }
                await ctx.SaveChangesAsync();
                StartLoadingMiniatures();
            }
        }

        private IList<Photo> DiscoverPhotosInDirectories(ICollection<Models.PhotoDirectories> directories)
        {
            List<string> supportedFormats = new List<string> { ".jpg", ".jpeg", ".bmp", ".png" };

            List<Photo> discoveredPhotos = new List<Photo>();
            foreach (var directoryPath in directories)
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath.Path);
                foreach (var fileinfo in directory.GetFiles().Where(f => supportedFormats.Contains(f.Extension.ToLower())))
                {
                    Photo photo = new Photo()
                    {
                        FullName = fileinfo.FullName,
                        Name = fileinfo.Name
                    };
                    if (ValidatePhoto(photo))
                    {
                        discoveredPhotos.Add(photo);
                    }
                }
            }
            return discoveredPhotos;
        }

        private async Task<IList<Face>> UploadToFaceApiAsync (string fullPath)
        {
            IList<Face> detectedFaces = new List<Face>();
            //Face[] faces = new Face[200];
            Console.WriteLine($"Uploading photo {Path.GetFileNameWithoutExtension(fullPath)} to faceApi");
            
            if (File.Exists(fullPath))
            {
                FileStream fileStream = new FileStream(fullPath, FileMode.Open); //File.OpenRead(fullPath);
                //detectedFaces = await faceClient.Face.DetectWithStreamAsync(fileStream);
                detectedFaces = await faceClient.DetectAsync(fileStream);
            }

            Console.WriteLine($"Done uploading photo to faceApi, detected {detectedFaces.Count} faces on that image");
            return detectedFaces;
        }

        private void ExtractFaceFromPhoto(Photo photo, DetectedFace detectedFace)
        {
            Bitmap image = new Bitmap(photo.FullName);
            Rectangle rect = new Rectangle
            {
                X = detectedFace.FaceRectangle.Left,
                Y = detectedFace.FaceRectangle.Top,
                Width = detectedFace.FaceRectangle.Width,
                Height = detectedFace.FaceRectangle.Height
            };
            using (Bitmap faceBitmap = image.Clone(rect, PixelFormat.Format24bppRgb))
            {
                var t = DirectoryHelper.GetFacePath(detectedFace.FaceId.ToString());
                detectedFace.FaceFile = t;
                faceBitmap.Save(t);
            }
        }

        private void Miniature_MiniatureLoaded()
        {
            StartLoadingMiniatures();
        }
        private void StartLoadingMiniatures()
        {
            bool successfulyLoaded = false;
            while (miniatureLoadQueue.Any() && !successfulyLoaded)
            {
                var id = miniatureLoadQueue.First();
                if (id < miniatures.Count)
                {
                    if (!loadedMiniatureIds.Contains(id))
                    {
                        successfulyLoaded = true;
                        miniatures[id].LoadImage();
                        loadedMiniatureIds.Add(id);
                    }
                    else
                    {
                        miniatureLoadQueue.RemoveAll(e => e == id);
                    }
                }
                else
                {
                    return;
                }
            }
        }
        

        private bool ValidatePhoto(Photo photo)
        {
            BitmapDecoder img = BitmapDecoder.Create(new Uri(photo.FullName), BitmapCreateOptions.None, BitmapCacheOption.None);
            var imgWidth = img.Frames[0].Width;
            var imgHeight = img.Frames[0].Height;
            if (imgWidth <= 5 || imgHeight <= 5)
            {
                return false;
            }
            return true;
        }

        private PhotoMiniature GetMiniatureForPhoto(Photo photo)
        {
            if (photo.FullName.Contains("IMG_20180310_165252.jpg"))
            {
                Console.WriteLine();
            }
            if (!DirectoryHelper.CheckIfMiniatureExistsForPhoto(photo.FullName))
            {
                Console.WriteLine($"Does not exist, generating...");
                return GenerateMiniature(photo);
                //ctx.MiniatureLocations.Add(miniature);
            }
            else
            {
                Console.WriteLine($"Miniature for photo {photo.Name} found.");
                var miniature = new PhotoMiniature();
                miniature.MiniatureFileName = DirectoryHelper.GetMiniatureName(photo.Name);
                miniature.OriginalImageFullPath = photo.FullName;
                miniature.MiniatureFullPath = DirectoryHelper.GetFullMiniaturePath(miniature.MiniatureFileName, false);
                miniature.OriginalImageFileName = photo.Name;
                return miniature;
            }

            /*var miniature = ctx.MiniatureLocations.Where(m => m.OriginalImageFullPath.Equals(photo.FullName)).FirstOrDefault();
            if (miniature == null)
            {
                miniature = new Miniature();
                Console.WriteLine($"Miniature for photo {photo.FullName} not found in DB, checking for miniature file...");

                if (!DirectoryHelper.CheckIfMiniatureExistsForPhoto(photo.FullName))
                {
                    Console.WriteLine($"Does not exist, generating...");
                    miniature = GenerateMiniature(photo);
                    ctx.MiniatureLocations.Add(miniature);
                }
                else
                {
                    Console.WriteLine($"Miniature for photo {photo.Name} found.");
                    miniature.MiniatureFileName = DirectoryHelper.GetMiniatureName(photo.Name);
                    miniature.OriginalImageFullPath = photo.FullName;
                    miniature.MiniatureFullPath = DirectoryHelper.GetFullMiniaturePath(miniature.MiniatureFileName, false);
                    miniature.OriginalImageFileName = photo.Name;
                    photo.Miniature = miniature;
                    ctx.MiniatureLocations.Add(miniature);
                }
            }
            else
            {
                Console.WriteLine($"Miniature for photo {photo.Name} found in DB, checking if exists.");
                if (!DirectoryHelper.CheckIfMiniatureExistsForPhoto(photo.FullName))
                {
                    Console.WriteLine($"Does not exist, generating...");
                    miniature = GenerateMiniature(photo);
                    photo.Miniature = miniature;
                    ctx.MiniatureLocations.Add(miniature);
                }
                else
                {
                    Console.WriteLine($"2Miniature for photo {photo.Name} found.");
                    miniature.MiniatureFileName = DirectoryHelper.GetMiniatureName(photo.Name);
                    miniature.OriginalImageFullPath = photo.FullName;
                    miniature.MiniatureFullPath = DirectoryHelper.GetFullMiniaturePath(miniature.MiniatureFileName, false);
                    miniature.OriginalImageFileName = photo.Name;
                }
            }*/
            //return miniature;
        }
        private PhotoMiniature GenerateMiniature(Photo photo)
        {
            miniatureGenerator.MaxResolution = 128;
            var miniatureImage = miniatureGenerator.GenerateMiniature(photo.FullName);

            var path = DirectoryHelper.GetFullMiniaturePath(photo.Name, true);

            var miniature = new PhotoMiniature()
            {
                MiniatureFullPath = path,
                MiniatureFileName = DirectoryHelper.GetMiniatureName(photo.Name),
                OriginalImageFullPath = photo.FullName,
                OriginalImageFileName = photo.Name
            };
            if (!File.Exists(path))
            {
                miniatureImage.Save(path);
            }
            return miniature;
        }
        private ICollection<string> FindDupePhotoNames(ICollection<Photo> photos)
        {
            HashSet<string> dupes = new HashSet<string>();
            foreach (var item in photos)
            {
                if (dupes.Contains(item.Name))
                {
                    dupes.Add(item.Name);
                }
            }
            return dupes;
        }
        
        public void ShowPreview(Photo photo)
        {
            var idx = photos.IndexOf(photo);
            if (preview != null)
            {
                preview.Close();
            }
            preview = new ImagePreview(photos, idx);
            
            preview.Show();
        }

        private void AddMiniatureToGrid(Photo photo)
        {
            ILoadableImage miniatureControl = new ClickableMiniatureImage(this, photo);
            miniatures.Add(miniatureControl);
            grid_images.Children.Add((ClickableMiniatureImage)miniatureControl);
        }
        private void ScrollViewMiniatures_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double itemsInRow = e.ViewportWidth / grid_images.ItemWidth;
            double itemsInColumn = e.ViewportHeight / grid_images.ItemHeight;
            itemsInColumn++;

            int visibleItems = (int)itemsInRow * (int)itemsInColumn;
            int rowOffset = (int)(e.VerticalOffset / grid_images.ItemHeight);

            for (int i = 0; i < visibleItems; i++)
            {
                int index = i + rowOffset * (int)itemsInRow;
                miniatureLoadQueue.Add(index);
            }
            StartLoadingMiniatures();
            //if (!miniaturesInitiated)
            //{
            //    miniaturesInitiated = true;
            //}

            //Console.WriteLine($"e.itemsInRow: {(int)itemsInRow}");
            //Console.WriteLine($"e.itemsInColumn: {(int)itemsInColumn}");
            //Console.WriteLine($"e.visibleItems: {visibleItems}");
            //Console.WriteLine($"e.rowOffset: {rowOffset}");

            //Console.WriteLine($"e.ViewportWidth: {e.ViewportWidth}");
            //Console.WriteLine($"e.VerticalOffset: {e.VerticalOffset}");
            //Console.WriteLine($"e.VerticalChange: {e.VerticalChange}");
        }
        private void Gallery_Closed(object sender, EventArgs e)
        {
            ctx.SaveChanges();
            ctx.Dispose();
        }
    }
}