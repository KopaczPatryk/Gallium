using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallium.Data
{
    public class PhotoMiniature
    {
        public int Id { get; set; }
        //public string OriginalImageFullPath { get; set; }
        public string OriginalImageFileName { get; set; }
        public string MiniatureFullPath { get; set; }
        public string MiniatureFileName { get; set; }
    }
}
