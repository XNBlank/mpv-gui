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

namespace mpv_gui
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void vidAutoCenterCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (vidAutoCenterCheck.IsChecked == true)
            {
                vidXBox.IsEnabled = false;
                vidYBox.IsEnabled = false;
            }
            else if(vidAutoCenterCheck.IsChecked == false)
            {
                vidXBox.IsEnabled = true;
                vidYBox.IsEnabled = true;
            }
        }
    }
}
