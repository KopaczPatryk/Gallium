using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gallium.UserControls
{
    class ExtendedComboBox : ComboBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            Console.WriteLine("ff");
            base.OnPreviewTextInput(e);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            Console.WriteLine("dd");
            base.OnTextInput(e);
        }
    }
}
