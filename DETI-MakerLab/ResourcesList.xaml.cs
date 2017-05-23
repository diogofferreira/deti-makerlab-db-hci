using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
        private SqlConnection cn;

        public ResourcesList()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            KitsListData = new ObservableCollection<Kit>();
            LoadResources();
            LoadKits();
            electronics_list.ItemsSource = EquipmentsListData;
            kits_list.ItemsSource = KitsListData;
        }

        private void LoadResources()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM ALL_ELECTRONIC_RESOURCES", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ElectronicResources Resource = new ElectronicResources(
                    reader["ProductName"].ToString(),
                    reader["Manufacturer"].ToString(),
                    reader["Model"].ToString(),
                    reader["ResDescription"].ToString(),
                    new Staff(
                        int.Parse(reader["EmployeeNum"].ToString()),
                        reader["FirstName"].ToString(),
                        reader["LastName"].ToString(),
                        reader["Email"].ToString(),
                        reader["StaffImage"].ToString()
                        ),
                    reader["PathToImage"].ToString()
                    );

                EquipmentsListData.Add(Resource);
            }

            cn.Close();
        }

        private void LoadKits()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM Kit", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Kit Resource = new Kit(
                    int.Parse(reader["ResourceID"].ToString()),
                    reader["KitDescription"].ToString()
                    );

                KitsListData.Add(Resource);
            }

            cn.Close();
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
        }

        private void kit_info_Click(object sender, RoutedEventArgs e)
        {
            Kit kit = (Kit)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goToKitPage(kit);
        }
    }
}
