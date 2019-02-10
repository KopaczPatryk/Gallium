using Gallium.Data;
using Gallium.Models;
using Gallium.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gallium.Windows
{
    /// <summary>
    /// Interaction logic for People.xaml
    /// </summary>
    public partial class People : Window
    {
        public People(IList<Person> peopleList, GalliumContext context)
        {
            InitializeComponent();
            foreach (var person in peopleList)
            {
                PeopleList.Children.Add(new DetailedPersonView(person, context));
            }
        }
    }
}
