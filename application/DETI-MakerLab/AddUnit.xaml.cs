using Ookii.Dialogs.Wpf;
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
using Xceed.Wpf.Toolkit;

namespace DETI_MakerLab
{

    // Helper class to wrap units
    public class UnitsHelper
    {
        private ResourceItem _resource;
        private int _units;
        private String _supplier;

        public ResourceItem Resource
        {
            get { return _resource; }
            set { _resource = value; }
        }

        public int Units
        {
            get { return _units; }
            set { _units = value; }
        }

        public String Supplier
        {
            get { return _supplier; }
            set { _supplier = value; }
        }

        public UnitsHelper(ResourceItem Resource, int Units, String Supplier)
        {
            this.Resource = Resource;
            this.Units = Units;
            this.Supplier = Supplier;
        }
    }

    public partial class AddUnit : Page, DMLPages
    {
        private SqlConnection cn;
        private List<ResourceItem> ResourceItems;
        private ObservableCollection<ResourceItem> EquipmentsListData;
        private Staff User;
        private List<UnitsHelper> Units;

        public AddUnit(Staff user)
        {
            InitializeComponent();
            User = user;
            ResourceItems = new List<ResourceItem>();
            EquipmentsListData = new ObservableCollection<ResourceItem>();
            Units = new List<UnitsHelper>();
            try
            {
                // Load resource items to show on units list
                LoadResources();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            units_list.ItemsSource = EquipmentsListData;
        }

        private bool addResourceItemUnit(ElectronicUnit unit)
        {
            // Get unit's equipment and add it another
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

        private void LoadResources()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.ALL_ELECTRONIC_UNITS", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ElectronicResources resource = new ElectronicResources(
                    reader["ProductName"].ToString(),
                    reader["Manufacturer"].ToString(),
                    reader["Model"].ToString(),
                    reader["ResDescription"].ToString(),
                    null,
                    reader["PathToImage"].ToString()
                    );

                // Check if the resource has units
                if (reader["ResourceID"] != DBNull.Value)
                {
                    ElectronicUnit unit = new ElectronicUnit(
                        int.Parse(reader["ResourceID"].ToString()),
                        resource,
                        reader["Supplier"].ToString()
                        );

                    // Add new resource item (and it's unit) to list, if it isn't there already
                    if (!addResourceItemUnit(unit))
                    {
                        ResourceItem ri = new ResourceItem(resource);
                        ri.addUnit(unit);
                        ResourceItems.Add(ri);
                    }
                } else
                {
                    ResourceItem ri = new ResourceItem(resource);
                    ResourceItems.Add(ri);
                }
            }

            cn.Close();

            foreach (ResourceItem ri in ResourceItems)
                EquipmentsListData.Add(ri);
        }

        private void ReadUnitsList()
        {
            Boolean added = false;

            foreach (ResourceItem resource in units_list.Items)
            {
                // Find each field of the listbox's template
                var container = units_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                int units = int.Parse(((DecimalUpDown)units_list.ItemTemplate.FindName("equipment_units", listBoxItemCP)).Text);
                if (units > 0)
                {
                    String supplier = ((TextBox)units_list.ItemTemplate.FindName("equipment_supplier", listBoxItemCP)).Text;
                    if (String.IsNullOrEmpty(supplier) || supplier.Equals("Supplier"))
                        throw new Exception("Invalid supplier for " + resource + " units!");
                    Units.Add(new UnitsHelper(resource, units, supplier));
                    added = true;
                }
            }
            if (!added)
                throw new Exception("You need to select at least one unit!");
        }

        private void AddUnits()
        {
            // Add units of each equipment
            foreach (UnitsHelper helper in Units)
                UpdateSingleEquipment(helper.Resource, helper.Units, helper.Supplier);
        }

        private void UpdateSingleEquipment(ResourceItem resourceItem, int units, String supplier)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            ElectronicResources resource = resourceItem.Resource;

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
            cmd.CommandText = "DML.ADD_UNITS";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
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
                ReadUnitsList();
                MessageBoxResult confirm = System.Windows.MessageBox.Show(
                    "Do you confirm the addition of the selected units?",
                    "Units Addition Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    AddUnits();
                    ResourceItems.Clear();
                    EquipmentsListData.Clear();
                    LoadResources();
                    System.Windows.MessageBox.Show("Units have been successfully added!");
                }

            } catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            } catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged_Equipments(object sender, TextChangedEventArgs e)
        {
            // Filter equipments which contains writed keyword
            if (EquipmentsListData.Count > 0 && !search_box_equipments.Text.Equals(""))
            {
                var filteredEquipments = EquipmentsListData.Where(i => ((ResourceItem)i).ToString().ToLower().Contains(search_box_equipments.Text.ToLower())).ToArray();
                units_list.ItemsSource = filteredEquipments;
            }
            else
            {
                units_list.ItemsSource = EquipmentsListData;
            }
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ResourceItem equipment = (ResourceItem)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goToEquipmentPage(equipment.Resource);
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
