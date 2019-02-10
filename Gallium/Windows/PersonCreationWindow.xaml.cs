using Gallium.Models;
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
    public delegate void OnPersonCreatedHandler(Person person);

    public partial class PersonCreationWindow : Window
    {
        public event OnPersonCreatedHandler OnPersonCreatedHandler;
        public PersonCreationWindow()
        {
            InitializeComponent();
        }

        private void CreatePerson_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PersonName.Text) && !string.IsNullOrWhiteSpace(PersonLastName.Text))
            {
                Person person = new Person();
                person.Name = PersonName.Text;
                person.LastName = PersonLastName.Text;
                if (BirthDate.SelectedDate.HasValue)
                {
                    person.DateOfBirth = BirthDate.SelectedDate;
                }
                OnPersonCreatedHandler(person);
                this.Close();
            }
        }
    }
}
