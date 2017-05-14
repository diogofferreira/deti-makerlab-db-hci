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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DETI_MakerLab
{
    /// <summary>
    /// Interaction logic for KitPage.xaml
    /// </summary>
    public partial class KitPage : Page
    {
        private Kit _kit;

        public KitPage(Kit kit)
        {
            InitializeComponent();
            _kit = kit;
            kit_name.Text = _kit.Description;
            content_list.ItemsSource = kit.Units;
        }

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goBack();
        }
    }
}
