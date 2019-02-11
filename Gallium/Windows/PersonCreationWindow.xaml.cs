using Gallium.Models;
using Microsoft.ProjectOxford.Face;
using System.Windows;

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
                Close();
            }
        }
    }
}
