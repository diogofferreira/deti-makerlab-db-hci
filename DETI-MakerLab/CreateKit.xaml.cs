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
                "Pi", "Model B", "Description", null, "none"));
            units_list.ItemsSource = EquipmentsListData;
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow) Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
        }
    }
}
