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
    /// Interaction logic for CreateKit.xaml
    /// </summary>
    public partial class CreateKit : Page
    {
        private SqlConnection cn;
        private List<ResourceItem> ResourceItems;
        private ObservableCollection<ElectronicResources> EquipmentsListData;

        private bool addResourceItemUnit(ElectronicUnit unit)
        {
            foreach (ResourceItem item in ResourceItems)
            {
                if (item.Resource.Equals(unit.Model))
                {
                    item.addUnit(unit);
                    return true;
                }
            }
            return false;
        }

        public CreateKit()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            ResourceItems = new List<ResourceItem>();

            // Hardcoded Data
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3",
            "Pi", "Model B", "Raspberry Description", null, "/images/rasp.png"));
            EquipmentsListData.Add(new ElectronicResources("Arduino Uno",
            "Adafruit", "Uno", "Arduino Description", null, "/images/ard.png"));
            units_list.ItemsSource = EquipmentsListData;
        }

        private void LoadResources()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand("RESOURCES_TO_REQUEST", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ElectronicResources resource = new ElectronicResources(
                    row["ProductName"].ToString(),
                    row["Manufactor"].ToString(),
                    row["Model"].ToString(),
                    row["Description"].ToString(),
                    null,
                    row["PathToImage"].ToString()
                    );

                ElectronicUnit unit = new ElectronicUnit(
                    int.Parse(row["ResourceID"].ToString()),
                    resource,
                    row["Supplier"].ToString()
                    );

                if (!addResourceItemUnit(unit))
                {
                    ResourceItem ri = new ResourceItem(resource);
                    ri.addUnit(unit);
                    ResourceItems.Add(ri);
                }
            }

            cn.Close();

            foreach (ResourceItem ri in ResourceItems)
                EquipmentsListData.Add(ri.Resource);
        }

        private void submitKitCreation()
        {
            List<ElectronicUnit> toRequest = new List<ElectronicUnit>();

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            foreach (ElectronicResources resource in units_list.Items)
            {
                ResourceItem ri = null;          
                foreach (ResourceItem r in ResourceItems)
                    if (r.Resource.Equals(resource)) {
                        ri = r;
                        break;
                    }


                var container = units_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                int units = int.Parse(((TextBox)units_list.ItemTemplate.FindName("equipment_units", listBoxItemCP)).Text);
                while (units > 0)
                {
                    ElectronicUnit unit = ri.requestUnit();
                    toRequest.Add(unit);
                    units--;
                }
            }

            SqlCommand cmd = new SqlCommand("CREATE_KIT (@KitDescription, @KitID)", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@KitID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@KitDescription", kit_name.Text);
            int kitID = -1;

            try
            {
                cmd.ExecuteNonQuery();
                kitID = Convert.ToInt32(cmd.Parameters["@KitID"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
            }
            finally
            {
                cn.Close();
            }

            if (kitID == -1)
                throw new Exception("Error creating kit");

            foreach (ElectronicUnit unit in toRequest)
            {
                cmd = new SqlCommand("ADD_UNIT_KIT (@KitID, @ResourceID)", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@KitID", kitID);
                cmd.Parameters.AddWithValue("@ResourceID", unit.ResourceID);

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

        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow) Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
        }

        private void create_kit_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                submitKitCreation();
                MessageBox.Show("Kit has been added!");
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                // TODO : create object and pass it to kit page
                //window.goToKitPage(kit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
