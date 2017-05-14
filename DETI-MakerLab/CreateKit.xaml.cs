using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CreateKit.xaml
    /// </summary>
    public partial class CreateKit : Page
    {
        private ObservableCollection<ElectronicResources> EquipmentsListData;

        public CreateKit()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            // Hardcoded Data
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3",
            "Pi", "Model B", "Raspberry Description", null, "images/rasp.png"));
            EquipmentsListData.Add(new ElectronicResources("Arduino Uno",
            "Adafruit", "Uno", "Arduino Description", null, "images/ard.png"));
            units_list.ItemsSource = EquipmentsListData;
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow) Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
        }

        private void create_kit_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kit has been added !");
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            // TODO : create object and pass it to kit page
            //window.goToKitPage(kit);
        }
    }
}
