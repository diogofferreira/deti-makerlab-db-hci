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
        private Staff User;

        public AddUnit(Staff user)
        {
            InitializeComponent();
            User = user;
            Console.WriteLine(user);
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            LoadEquipments();
            units_list.ItemsSource = EquipmentsListData;
        }

        private void LoadEquipments()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM ELECTRONIC_RESOURCES_INFO", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ElectronicResources Resource = new ElectronicResources (
                    reader["ProductName"].ToString(),
                    reader["Manufacturer"].ToString(),
                    reader["Model"].ToString(),
                    reader["ResDescription"].ToString(),
                    new Staff (
                        int.Parse(reader["EmployeeNum"].ToString()),
                        reader["FirstName"].ToString(),
                        reader["LastName"].ToString(),
                        reader["Email"].ToString(),
                        reader["StaffImage"].ToString()
                        ),
                    reader["ResImage"].ToString()
                    );
                EquipmentsListData.Add(Resource);
            }

            cn.Close();
        }

        private void UpdateUnits()
        {
            foreach (ElectronicResources resource in units_list.Items)
            {
                var container = units_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                int units = int.Parse(((TextBox)units_list.ItemTemplate.FindName("equipment_units", listBoxItemCP)).Text);
                if (units > 0)
                {
                    String supplier = ((TextBox)units_list.ItemTemplate.FindName("equipment_supplier", listBoxItemCP)).Text;
                    UpdateSingleEquipment(resource, units, supplier);
                }
            }            
        }

        private void UpdateSingleEquipment(ElectronicResources resource, int units, String supplier)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductName", resource.ProductName);
            cmd.Parameters.AddWithValue("@Manufacturer", resource.Manufactor);
            cmd.Parameters.AddWithValue("@Model", resource.Model);
            cmd.Parameters.AddWithValue("@Supplier", supplier);
            cmd.Parameters.AddWithValue("@Units", units);
            cmd.Parameters.AddWithValue("@EmployeeID", User.EmployeeNum);
            cmd.CommandText = "dbo.ADD_UNITS";

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

        private void add_units_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateUnits();
                MessageBox.Show("Unit has been added!");
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                // TODO : create object and pass it to kit page
                //window.goToKitPage(kit);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void TextBox_TextChanged_Equipments(object sender, TextChangedEventArgs e)
        {
            // Filter equipments which contains writed keyword
            if (EquipmentsListData.Count > 0 && !search_box_equipments.Text.Equals(""))
            {
                var filteredEquipments = EquipmentsListData.Where(i => ((Resources)i).ToString().ToLower().Contains(search_box_equipments.Text.ToLower())).ToArray();
                units_list.ItemsSource = filteredEquipments;
            }
            else
            {
                units_list.ItemsSource = EquipmentsListData;
            }
        }
    }
}
