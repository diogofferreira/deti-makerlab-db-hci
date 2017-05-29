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
    /// <summary>
    /// Interaction logic for Electronics.xaml
    /// </summary>
    public partial class Electronics : Page, DMLPages
    {
        SqlConnection cn;
        private int _userID;
        private ObservableCollection<Project> ProjectsListData;
        private ObservableCollection<ListItem> ResourcesListData;
        private ObservableCollection<Resources> ActiveRequisitionsData;
        private List<ResourceItem> ResourceItems;
        private List<KitItem> KitItems;
        private Project _selectedProject;

        internal int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        internal Project SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
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

        private bool addKitItemUnit(Kit unit)
        {
            foreach (KitItem item in KitItems)
            {
                if (item.KitDescription.Equals(unit.Description))
                {
                    item.addUnit(unit);
                    return true;
                }
            }
            return false;
        }

        public Electronics(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
            this.SelectedProject = null;
            ProjectsListData = new ObservableCollection<Project>();
            ResourcesListData = new ObservableCollection<ListItem>();
            ActiveRequisitionsData = new ObservableCollection<Resources>();
            ResourceItems = new List<ResourceItem>();
            KitItems = new List<KitItem>();
            try
            {
                LoadProjects();
                LoadAvailableResources();
                LoadProjectActiveRequisitons();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            projects_list.ItemsSource = ProjectsListData;
            equipment_list.ItemsSource = ResourcesListData;
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            try
            {
                ResourceItem equipment = (ResourceItem)(sender as Button).DataContext;
                window.goToEquipmentPage(equipment.Resource);
            } catch (Exception exc)
            {
                KitItem kit = (KitItem)(sender as Button).DataContext;
                window.goToKitPage(kit.Units[0]);
            }
        }

        private void LoadProjects()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");
            Console.WriteLine(UserID);
            SqlCommand cmd = new SqlCommand("SELECT * FROM USER_PROJECTS (@nmec)", cn);
            cmd.Parameters.AddWithValue("@nmec", UserID);
            SqlDataReader reader = cmd.ExecuteReader();
            projects_list.Items.Clear();

            while (reader.Read())
            {
                Class cl = null;
                if (reader["ClassID"] != DBNull.Value)
                    cl = new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    );

                ProjectsListData.Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    cl
                    ));
            }
            cn.Close();
        }

        private void LoadAvailableResources()
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
                    Console.WriteLine(resource);
                    ResourceItem ri = new ResourceItem(resource);
                    ri.addUnit(unit);
                    ResourceItems.Add(ri);
                }
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                Kit unit = new Kit(
                    int.Parse(row["ResourceID"].ToString()),
                    row["KitDescription"].ToString()
                    );

                if (!addKitItemUnit(unit))
                {
                    KitItem ki = new KitItem(unit.Description);
                    ki.addUnit(unit);
                    KitItems.Add(ki);
                }
            }

            foreach (ResourceItem ri in ResourceItems)
                ResourcesListData.Add(ri);

            foreach (KitItem ki in KitItems)
                ResourcesListData.Add(ki);
        }

        private void LoadProjectActiveRequisitons()
        {
            if (SelectedProject == null)
                return;

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@pID", SelectedProject.ProjectID);
            cmd.CommandText = "dbo.PROJECT_ACTIVE_REQS";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ActiveRequisitionsData.Add(new ElectronicUnit(
                    int.Parse(row["ResourceID"].ToString()),
                    new ElectronicResources(
                        row["ProductName"].ToString(),
                        row["Manufacturer"].ToString(),
                        row["Model"].ToString(),
                        row["ResDescription"].ToString(),
                        null,
                        row["PathToImage"].ToString()
                        ),
                    row["Supplier"].ToString()
                    ));
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                ActiveRequisitionsData.Add(new Kit(
                    int.Parse(row["ResourceID"].ToString()),
                    row["KitDescription"].ToString()
                    ));
            }
        }

        private void SubmitRequisitionResources()
        {
            int reqID = -1;

            if (SelectedProject == null)
                throw new Exception("You have to select a project first!");

            List<Resources> toBeRequested = new List<Resources>();
            List < ListItem > toBeRemoved = new List<ListItem>();

            DataTable toRequest = new DataTable();
            toRequest.Clear();
            toRequest.Columns.Add("ResourceID", typeof(int));

            foreach (ListItem resource in equipment_list.Items)
            {
                var container = equipment_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                DecimalUpDown unitsTextBox = equipment_list.ItemTemplate.FindName("equipment_units", listBoxItemCP) as DecimalUpDown;
                int units = int.Parse(unitsTextBox.Text);
                if (units == 0)
                    continue;
                
                unitsTextBox.Value = 0;

                while(units > 0) { 
                    Resources unit = null;
                    if (resource is ResourceItem)
                    {
                        ResourceItem r = resource as ResourceItem;
                        if (units > r.Units.Count)
                            throw new Exception("You cannot request more units than available!");
                        unit = r.requestUnit();
                        if (r.Units.Count == 0)
                            toBeRemoved.Add(r);
                    }
                    else if (resource is KitItem)
                    {
                        KitItem k = resource as KitItem;
                        if (units > k.Units.Count)
                            throw new Exception("You cannot request more units than available!");
                        unit = k.requestUnit();
                        if (k.Units.Count == 0)
                            toBeRemoved.Add(k);
                    }
                    toBeRequested.Add(unit);
                    DataRow row = toRequest.NewRow();
                    row["ResourceID"] = unit.ResourceID;
                    toRequest.Rows.Add(row);
                    units--;
                }
            }

            if (toRequest.Rows.Count == 0)
                throw new Exception("You can't request 0 resources!");

            reqID = SubmitRequisition();
            if (reqID == -1)
                throw new Exception("Error creating requisition");

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            SqlParameter listParam = cmd.Parameters.AddWithValue("@UnitsList", toRequest);
            listParam.SqlDbType = SqlDbType.Structured;
            cmd.Parameters.AddWithValue("@reqID", reqID);
            cmd.CommandText = "dbo.REQUEST_RESOURCES";

            try
            {
                cmd.ExecuteNonQuery();
                foreach (Resources resource in toBeRequested)
                    ActiveRequisitionsData.Add(resource);
                foreach (ListItem li in toBeRemoved)
                {
                    ResourcesListData.Remove(li);
                    if (typeof(ResourceItem).IsInstanceOfType(li))
                    {
                        ResourceItem ri = li as ResourceItem;
                        ResourceItems.Remove(ri);
                    }
                    else if (typeof(KitItem).IsInstanceOfType(li))
                    {
                        KitItem ki = li as KitItem;
                        KitItems.Remove(ki);
                    }
                }
                equipment_list.Items.Refresh();
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

        private int SubmitRequisition()
        {
            int reqID = -1;
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                return reqID;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@RequisitionID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@ProjectID", SelectedProject.ProjectID);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.CommandText = "dbo.CREATE_REQUISITION";

            try
            {
                cmd.ExecuteNonQuery();
                reqID = Convert.ToInt32(cmd.Parameters["@RequisitionID"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return reqID;
        }

        private void SubmitDeliveryResources()
        {
            int delID = -1;

            if (SelectedProject == null)
                return;

            DataTable toDeliver = new DataTable();
            toDeliver.Clear();
            toDeliver.Columns.Add("ResourceID", typeof(int));
            List<Resources> toDelete = new List<Resources>();

            foreach (Resources resource in active_requisitions_list.Items)
            {
                var container = active_requisitions_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                if (!((CheckBox)active_requisitions_list.ItemTemplate.FindName("active_checkbox", listBoxItemCP)).IsChecked ?? false)
                    continue;

                toDelete.Add(resource);
                DataRow row = toDeliver.NewRow();
                row["ResourceID"] = resource.ResourceID;
                toDeliver.Rows.Add(row);
            }

            if (toDeliver.Rows.Count == 0)
                throw new Exception("You can't deliver 0 resources!");

            delID = SubmitDelivery();
            if (delID == -1)
                throw new Exception("Error creating delivery");

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            SqlParameter listParam = cmd.Parameters.AddWithValue("@UnitsList", toDeliver);
            listParam.SqlDbType = SqlDbType.Structured;
            cmd.Parameters.AddWithValue("@delID", delID);
            cmd.CommandText = "dbo.DELIVER_RESOURCES";

            try
            {
                cmd.ExecuteNonQuery();
                foreach (Resources r in toDelete)
                {
                    ActiveRequisitionsData.Remove(r);
                    
                    if (typeof(ElectronicUnit).IsInstanceOfType(r))
                    {
                        ElectronicUnit unit = r as ElectronicUnit;
                        if (!addResourceItemUnit(unit))
                        {
                            ResourceItem ri = new ResourceItem(unit.Model);
                            ri.addUnit(unit);
                            ResourceItems.Add(ri);
                            ResourcesListData.Add(ri);
                        }
                    }
                    else if (typeof(Kit).IsInstanceOfType(r))
                    {
                        Kit unit = r as Kit;
                        if (!addKitItemUnit(unit))
                        {
                            KitItem ki = new KitItem(unit.Description);
                            ki.addUnit(unit);
                            KitItems.Add(ki);
                            ResourcesListData.Add(ki);
                        }
                    }
                }
                equipment_list.Items.Refresh();
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

        private int SubmitDelivery()
        {
            int delID = -1;
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                return delID;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@DeliveryID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@ProjectID", SelectedProject.ProjectID);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.CommandText = "dbo.CREATE_DELIVERY";

            try
            {
                cmd.ExecuteNonQuery();
                delID = Convert.ToInt32(cmd.Parameters["@DeliveryID"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return delID;
        }

        private void request_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkProject();
                MessageBoxResult confirm = System.Windows.MessageBox.Show(
                    "Do you confirm this requisition?",
                    "Requisition Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    SubmitRequisitionResources();
                    System.Windows.MessageBox.Show("Requisition done with success!");
                }
                
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deliver_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkProject();
                MessageBoxResult confirm = System.Windows.MessageBox.Show(
                    "Do you confirm this delivery?",
                    "Delivery Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    SubmitDeliveryResources();
                    System.Windows.MessageBox.Show("Delivery done with success!");
                }
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void checkProject()
        {
            foreach (Project resource in projects_list.Items)
            {
                var container = projects_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                RadioButton button = (RadioButton)projects_list.ItemTemplate.FindName("project_button", listBoxItemCP);

                if (button.IsChecked == true)
                    SelectedProject = resource;
            }
        }

        private void project_button_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Find selected project
                checkProject();

                // Enable buttons
                request_button.IsEnabled = true;
                deliver_button.IsEnabled = true;

                equipment_list.IsEnabled = true;
                
                // Clear active requisitions data and load the active requisitions for selected project
                ActiveRequisitionsData.Clear();
                LoadProjectActiveRequisitons();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged_Equipments(object sender, TextChangedEventArgs e)
        {
            // Filter resources which contains writed keyword
            if (ResourcesListData.Count > 0 && !search_box_equipments.Text.Equals(""))
            {
                var filteredResources = ResourcesListData.Where(i => ((ListItem)i).ToString().ToLower().Contains(search_box_equipments.Text.ToLower())).ToArray();
                equipment_list.ItemsSource = filteredResources;
            }
            else
            {
                equipment_list.ItemsSource = ResourcesListData;
            }
        }

        private void TextBox_TextChanged_Requisitions(object sender, TextChangedEventArgs e)
        {
            // Filter requisitions which contains writed keyword
            if (ActiveRequisitionsData.Count > 0 && !search_box_requisitions.Text.Equals(""))
            {
                var filteredRequisitions = ActiveRequisitionsData.Where(i => ((Resources)i).ToString().ToLower().Contains(search_box_requisitions.Text.ToLower())).ToArray();
                active_requisitions_list.ItemsSource = filteredRequisitions;
            }
            else
            {
                active_requisitions_list.ItemsSource = ActiveRequisitionsData;
            }
        }

        private void TextBox_TextChanged_Projects(object sender, TextChangedEventArgs e)
        {
            // Filter projects which contains writed keyword
            if (ProjectsListData.Count > 0 && !search_box_projects.Text.Equals(""))
            {
                var filteredProjects = ProjectsListData.Where(i => ((Project)i).ProjectName.ToLower().Contains(search_box_projects.Text.ToLower())).ToArray();
                projects_list.ItemsSource = filteredProjects;
            }
            else
            {
                projects_list.ItemsSource = ProjectsListData;
            }
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
