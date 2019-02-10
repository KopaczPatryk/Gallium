using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gallium.Core
{
    class MiniatureGenerator
    {
        public int MaxResolution { get; set; }

        public Bitmap GenerateMiniature (string path)
        {
            BitmapDecoder img = BitmapDecoder.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.None);
            var imgWidth = img.Frames[0].Width;
            var imgHeight = img.Frames[0].Height;

            double desX;
            double desY;
            double ratio = imgWidth / imgHeight;
            //wider
            if (ratio > 1)
            {
                desX = MaxResolution;
                desY = imgHeight / (imgWidth / MaxResolution);
            }
            else //thinner
            {
                desX = imgWidth / (imgHeight / MaxResolution);
                desY = MaxResolution;
            }
            using (var bmp = Bitmap.FromFile(path))
            {
                Bitmap scaledMiniature = new Bitmap(bmp, (int)desX, (int)desY);
                return scaledMiniature;
            }
        }
        
    }
}
