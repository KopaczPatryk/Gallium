using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallium.Helpers
{
    class DirectoryHelper
    {
        public const string MiniaturesFolder = @"miniatures";
        public const string FacesFolder = @"faces";
        public const string MiniaturesPrefix = "mini_";
        public const string FacesPrefix = "face_";

        private static readonly string baseDirectory = Properties.Settings.Default.GalleryMainFolder;

        public static string GetBaseDir()
        {
            return baseDirectory;
        }

        public static string GetFacePath(string name)
        {
            var path = Path.Combine(baseDirectory, FacesFolder, $"{FacesPrefix}{name}.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        public static string GetMiniatureName (string photoName)
        {
            return $"{MiniaturesPrefix}{photoName}";
        }

        public static bool CheckIfMiniatureExistsForPhoto (string photoFullPath)
        {
            var filename = Path.GetFileName(photoFullPath);
            var directory = Path.GetDirectoryName(photoFullPath);
            var resultingPath = Path.Combine(baseDirectory, MiniaturesFolder, $"{MiniaturesPrefix}{filename}");
            Console.WriteLine(resultingPath);
            return File.Exists(resultingPath);
        }

        public static string GetFullMiniaturePath(string miniatureName, bool appendPrefix)
        {
            string path = baseDirectory;
            CreateMiniaturesFolder();
            if (appendPrefix)
            {
                path = Path.Combine(baseDirectory, MiniaturesFolder, $"{ MiniaturesPrefix }{ miniatureName }");
            }
            else
            {
                path = Path.Combine(baseDirectory, MiniaturesFolder, $"{ miniatureName }");
            }
            return path;
        }

        private static void CreateMiniaturesFolder()
        {
            var path = Path.Combine(baseDirectory, MiniaturesFolder);
            Directory.CreateDirectory(path);
        }
        
    }
}
