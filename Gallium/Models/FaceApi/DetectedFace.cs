using Gallium.Data;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gallium.Models.FaceApi
{
    public class DetectedFace
    {
        public int Id { get; set; }
        public Guid FaceId { get; set; }
        public bool HumanVerified { get; set; }
        public bool AssignedByHuman { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public string FaceFile { get; set; }
        public bool IsValidFace { get; set; }

        public Person FaceOwner { get; set; }

        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public bool Postponed { get; internal set; }
    }
}
