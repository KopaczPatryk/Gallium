using Gallium.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallium.Models.FaceApi
{
    class PhotoEqualityComparer : IEqualityComparer<Photo>
    {
        public bool Equals(Photo x, Photo y)
        {
            if (x.FullName.Equals(y.FullName) || x.Name.Equals(y.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Photo obj)
        {
            var fnHash = obj.FullName.GetHashCode();
            var nHash = obj.Name.GetHashCode();
            return fnHash ^ nHash;
        }
    }
}
