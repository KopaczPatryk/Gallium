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
    /// <summary>
    /// Interaction logic for AutoCompleteTextBox.xaml
    /// </summary>
    public partial class AutoCompleteTextBox : UserControl
    {
        IList<string> allItems = new List<string>();

        //public AutoCompleteTextBox()
        //{
        //    InitializeComponent();
        //    SearchBox.TextChanged += SearchBox_TextChanged;
        //    ComboBox.DropDownClosed += ComboBox_DropDownClosed;
        //}

        //private void ComboBox_DropDownClosed(object sender, EventArgs e)
        //{
        //    ComboBox.Visibility = Visibility.Collapsed;
        //    SearchBox.Text = ComboBox.Text;
        //}

        //public void SetSource (IList<string> strings)
        //{
        //    allItems = strings;
        //    ComboBox.ItemsSource = new BindingList<string>(strings);
        //}

        //private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var filteredItems = allItems.Where(item => item.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
        //    ComboBox.ItemsSource = new BindingList<string>(filteredItems);

        //    if (filteredItems.Any())
        //    {
        //        ComboBox.Visibility = Visibility.Visible;
        //        ComboBox.IsDropDownOpen = true;
        //    }
        //    else
        //    {
        //        ComboBox.Visibility = Visibility.Collapsed;
        //        //ComboBox.IsDropDownOpen = false;
        //    }
        //}

        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }
        public void SetSource(IList<string> strings)
        {
            allItems = strings;
            SearchBox.TextChanged += SearchBox_TextChanged;
            Suggestions.ItemsSource = new BindingList<string>(strings);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filteredItems = allItems.Where(item => item.ToLower().Contains(SearchBox.Text.ToLower())).ToList();

            foreach (var item in filteredItems)
            {
                
            }
        }
    }
}
