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

namespace DETI_MakerLab
{
    /// <summary>
    /// Interaction logic for AddUnit.xaml
    /// </summary>
    public partial class AddUnit : Page
    {
        private SqlConnection cn;
        private List<ResourceItem> ResourceItems;
        private ObservableCollection<ResourceItem> EquipmentsListData;
        private Staff User;

        public AddUnit(Staff user)
        {
            InitializeComponent();
            User = user;
            Console.WriteLine(user);
            ResourceItems = new List<ResourceItem>();
            EquipmentsListData = new ObservableCollection<ResourceItem>();
            try
            {
                LoadEquipments();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            units_list.ItemsSource = EquipmentsListData;
        }

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

        private void LoadEquipments()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.CommandText = "dbo.RESOURCES_TO_REQUEST";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ElectronicResources resource = new ElectronicResources(
                    row["ProductName"].ToString(),
                    row["Manufacturer"].ToString(),
                    row["Model"].ToString(),
                    row["ResDescription"].ToString(),
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
                EquipmentsListData.Add(ri);
        }

        private void UpdateUnits()
        {
            Boolean added = false;
            foreach (ResourceItem resource in units_list.Items)
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
                    added = true;
                }
            }
            if (!added)
                throw new Exception("You need to select at least one unit!");
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
            cmd.CommandText = "dbo.ADD_UNITS";

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
                UpdateUnits();
                MessageBox.Show("Unit has been added!");
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                // TODO : create object and pass it to kit page
                //window.goToKitPage(kit);
            } catch (SqlException ex)
            {
                Helpers.ShowCustomDialogBox(ex);
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
    }
}
