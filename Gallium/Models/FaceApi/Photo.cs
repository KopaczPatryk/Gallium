using Gallium.Models.FaceApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallium.Data
{
    public class Photo
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }

        public PhotoMiniature Miniature { get; set; }

        public bool HasFaces { get; set; } = true;
        public virtual ICollection<DetectedFace> DetectedFaces { get; set; }
    }
}
