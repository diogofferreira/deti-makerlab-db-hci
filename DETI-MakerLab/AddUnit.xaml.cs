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
    /// Interaction logic for AddUnit.xaml
    /// </summary>
    public partial class AddUnit : Page
    {
        private SqlConnection cn;
        private ObservableCollection<ElectronicResources> EquipmentsListData;

        public AddUnit()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            //LoadEquipments();
            // Hardcoded Data
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3",
                "Pi", "Model B", "Description", null, "images/rasp.png"));
            EquipmentsListData.Add(new ElectronicResources("Arduino Uno",
            "Adafruit", "Uno", "Arduino Description", null, "images/ard.png"));
            units_list.ItemsSource = EquipmentsListData;
        }

        private void LoadEquipments()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM ElectronicResource", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            units_list.Items.Clear();

            while (reader.Read())
            {
                ElectronicResources Resource = new ElectronicResources(
                    reader["ProductName"].ToString(),
                    reader["Manufacturer"].ToString(),
                    reader["Model"].ToString(),
                    reader["Description"].ToString(),
                    null,
                    reader["PathToImage"].ToString()
                    );
                units_list.Items.Add(Resource);
            }

            cn.Close();
        }

        private void UpdateUnits()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            /*
            foreach (ListBoxItem resource in units_list.Items)
            {
                
                if (resource.units != 0)
                    UpdateSingleEquipment(resource.resource, resource.units, resource.supplier)
            }
            */
        }

        private void UpdateSingleEquipment(ElectronicResources resource, int units, String supplier)
        {
            int i = 0;
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "INSERT INTO ElectronicUnit (ProductName, Manufacturer, Model, Supplier) VALUES ";
            for (i = 0; i < units; i++)
            {
                if (i != 0)
                    cmd.CommandText += ", ";

                cmd.CommandText += "(" +
                    resource.ProductName + ", " +
                    resource.Manufactor + ", " +
                    resource.Model + ", " +
                    supplier + ")";
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private SqlConnection getSGBDConnection()
        {
            return new SqlConnection("data source= DESKTOP-H41EV9L\\SQLEXPRESS;integrated security=true;initial catalog=Northwind");
        }

        private bool verifySGBDConnection()
        {
            if (cn == null)
                cn = getSGBDConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        private void add_units_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Unit has been added!");
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            // TODO : create object and pass it to kit page
            //window.goToKitPage(kit);
        }
    }
}
