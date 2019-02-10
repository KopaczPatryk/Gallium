using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallium.Models
{
    public class Person
    {
        public int Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public override string ToString()
        {
            return $"{Name} {LastName} {DateOfBirth?.ToString("dd-MM-yyyy")}";
        }
    }
}
