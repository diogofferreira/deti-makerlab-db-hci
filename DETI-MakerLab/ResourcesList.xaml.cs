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
    /// Interaction logic for ResourcesList.xaml
    /// </summary>
    public partial class ResourcesList : Page
    {
        private ObservableCollection<ElectronicResources> EquipmentsListData;
        private ObservableCollection<Kit> KitsListData;


        public ResourcesList()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            KitsListData = new ObservableCollection<Kit>();
            // Hardcoded Data
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3", "Pi", "Model B", "Description", null, "none"));
            electronics_list.ItemsSource = EquipmentsListData;
            KitsListData.Add(new Kit(1, "Kit RaspberryPi"));
            kits_list.ItemsSource = KitsListData;
        }
    }
}
