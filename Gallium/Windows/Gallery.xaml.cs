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

        IList<Photo> photos = new List<Photo>();
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
                List<PhotoDirectories> directories = await ctx.Directories.ToListAsync();
                photos = PhotoHelper.DiscoverPhotosInDirectories(directories);
                

                photos = await TryGetMiniaturesAsync(photos, ctx);

                foreach (var photo in photos)
                {
                    AddMiniatureToGrid(photo);
                }

                foreach (ILoadableImage miniature in miniatures)
                {
                    miniature.MiniatureLoaded += Miniature_MiniatureLoaded;
                }
                StartLoadingMiniatures();
            }
        }
        

        
        private async Task<IList<Photo>> TryGetMiniaturesAsync(IList<Photo> photos, GalliumContext ctx)
        {
            photos = await ctx.Photos.Include(faceClient => faceClient.DetectedFaces.Select(df => df.FaceOwner)).Include(m => m.Miniature).Include(f => f.DetectedFaces).ToListAsync();
            foreach (var photo in photos)
            {
                photo.Miniature = GetMiniatureForPhoto(photo);
            }
            await ctx.SaveChangesAsync();
            return photos;
        }

        private PhotoMiniature GetMiniatureForPhoto(Photo photo)
        {
            if (!DirectoryHelper.CheckIfMiniatureExistsForPhoto(photo.FullName))
            {
                Console.WriteLine($"Does not exist, generating...");
                return GenerateMiniature(photo);
            }
            else
            {
                Console.WriteLine($"Miniature for photo {photo.Name} found.");
                var miniature = new PhotoMiniature();

                miniature.MiniatureFileName = DirectoryHelper.GetMiniatureName(photo.Name);
                //miniature.OriginalImageFullPath = photo.FullName;
                miniature.MiniatureFullPath = DirectoryHelper.GetFullMiniaturePath(miniature.MiniatureFileName, false);
                miniature.OriginalImageFileName = photo.Name;
                return miniature;
            }
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
                //OriginalImageFullPath = photo.FullName,
                OriginalImageFileName = photo.Name
            };
            if (!File.Exists(path))
            {
                miniatureImage.Save(path);
            }
            return miniature;
        }

        private void AddMiniatureToGrid(Photo photo)
        {
            ILoadableImage miniatureControl = new ClickableMiniatureImage(this, photo);
            miniatures.Add(miniatureControl);
            grid_images.Children.Add((ClickableMiniatureImage)miniatureControl);
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
