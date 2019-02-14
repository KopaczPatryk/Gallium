using Gallium.Data;
using Gallium.Models;
using Gallium.Models.FaceApi;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gallium.Helpers
{
    class PhotoHelper
    {
        public static IList<Photo> DiscoverPhotosInDirectories(ICollection<PhotoDirectories> directories)
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

        public static bool ValidatePhoto(Photo photo)
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

        public static async Task<IList<Face>> UploadToFaceApiAsync(string fullPath)
        {
            IList<Face> detectedFaces = new List<Face>();
            Console.WriteLine($"Uploading photo {Path.GetFileNameWithoutExtension(fullPath)} to faceApi");

            if (File.Exists(fullPath))
            {
                FileStream fileStream = new FileStream(fullPath, FileMode.Open); //File.OpenRead(fullPath);
                //detectedFaces = await faceClient.Face.DetectWithStreamAsync(fileStream);
                var faceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);
                detectedFaces = await faceClient.DetectAsync(fileStream);
            }

            Console.WriteLine($"Detected {detectedFaces.Count} faces on that image");
            return detectedFaces;
        }

        public static void ExtractFaceFromPhoto(Photo photo, DetectedFace detectedFace)
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
    }
}
