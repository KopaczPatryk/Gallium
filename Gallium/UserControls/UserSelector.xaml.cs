using Gallium.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gallium.UserControls
{
    public delegate void PersonSelectedEventHandler(Person selectedPerson);
    public partial class UserSelector : UserControl
    {
        public event PersonSelectedEventHandler PersonSelected;
        private IList<Person> people;
        private Person selectedPerson;

        public UserSelector()
        {
            InitializeComponent();
            TextBox_Search.TextChanged += TextBox_Search_TextChanged;
            ComboBox.DropDownClosed += ComboBox_DropDownClosed;
            ComboBox.DropDownOpened += ComboBox_DropDownOpened;

            Button_Clear.Click += Button_Clear_Click;
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            TextBox_Search.Focus();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (PersonSelected != null)
            {
                PersonSelected(null);
            }
            TextBox_Search.Text = "";
        }
        
        private void TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchWords = TextBox_Search.Text.Split(' ').ToList();
            List<Person> filteredPeople = new List<Person>(people);
            foreach (var word in searchWords)
            {
                filteredPeople = filteredPeople.Where(p => $"{p.Name} {p.LastName}".ToLower().Contains(word.ToLower())).ToList();
            }
            if (e.Changes.ElementAt(0).AddedLength > 0)
            {

            }
            ComboBox.IsDropDownOpen = true;
            ComboBox.ItemsSource = filteredPeople;
                TextBox_Search.Focus();
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            TextBox_Search.TextChanged -= TextBox_Search_TextChanged;
            selectedPerson = (Person)ComboBox.SelectedItem;
            if (selectedPerson != null)
            {
                if (PersonSelected != null)
                {
                    PersonSelected(selectedPerson);
                }
                int age = DateTime.Now.Subtract(selectedPerson.DateOfBirth.GetValueOrDefault()).Days / 360;
                TextBox_Search.Text = $"{selectedPerson.Name} {selectedPerson.LastName}".Trim();
            }
            TextBox_Search.TextChanged += TextBox_Search_TextChanged;
            TextBox_Search.Focus();

        }

        public void SetPeople(IList<Person> people)
        {
            this.people = people;
            ComboBox.ItemsSource = people;
        }
    }
}
