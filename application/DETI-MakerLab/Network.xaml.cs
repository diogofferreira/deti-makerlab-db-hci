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
    /// Interaction logic for Network.xaml
    /// </summary>
    public partial class Network : Page, DMLPages
    {
        private ObservableCollection<Project> ProjectsListData;
        private ObservableCollection<EthernetSocket> SocketsListData;
        private ObservableCollection<NetworkResources> ActiveRequisitionsData;
        private ObservableCollection<OS> OSList;
        private SqlConnection cn;
        private Project selectedProject;
        private WirelessLAN currentWLAN;

        private int _userID;

        public Network(int UserID)
        {
            InitializeComponent();
            this._userID = UserID;
            see_more.Visibility = Visibility.Hidden;
            ProjectsListData = new ObservableCollection<Project>();
            SocketsListData = new ObservableCollection<EthernetSocket>();
            ActiveRequisitionsData = new ObservableCollection<NetworkResources>();
            OSList = new ObservableCollection<OS>();
            try
            {
                // Load OS's, projects and sockets
                LoadOS();
                LoadProjects(UserID);
                LoadAvailableSockets();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            projects_list.ItemsSource = ProjectsListData;
            os_list.ItemsSource = OSList;
            socket_list.ItemsSource = SocketsListData;
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
        }

        private void LoadProjects(int UserID)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.USER_PROJECTS (@nmec)", cn);
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

        private void LoadOS()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.OS", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            OSList.Add(new OS(-1, "None"));

            while (reader.Read())
            {
                OSList.Add(new OS(
                    int.Parse(reader["OSID"].ToString()), 
                    reader["OSName"].ToString())
                    );
            }

            cn.Close();
        }

        private OS getOS(int OSID)
        {
            foreach (OS os in OSList)
                if (os.OSID == OSID)
                    return os;
            return null;
        }

        private void LoadAvailableSockets()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");
            
            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.AVAILABLE_SOCKETS ()", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                SocketsListData.Add(new EthernetSocket(
                    -1, 
                    null, 
                    int.Parse(reader["SocketNum"].ToString())
                    ));

            cn.Close();
        }

        private void LoadProjectActiveRequisitions()
        {
            if (selectedProject == null)
                throw new Exception("Select a project first!");
            // Load all possible network's resources requisition's for the selected project
            loadVMs();
            loadSockets();
            loadWLANs();
        }

        private void loadVMs()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.VM_INFO (@ProjectID)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", selectedProject.ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ActiveRequisitionsData.Add(new VirtualMachine(
                    int.Parse(reader["NetResID"].ToString()),
                    selectedProject,
                    reader["IP"].ToString(),
                    reader["PasswordHash"].ToString(),
                    reader["DockerID"].ToString(),
                    getOS(int.Parse(reader["OSID"].ToString()))
                    ));
            }

            cn.Close();
        }

        private void loadSockets()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.SOCKETS_INFO (@ProjectID)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", selectedProject.ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ActiveRequisitionsData.Add(new EthernetSocket(
                    int.Parse(reader["NetResID"].ToString()),
                    selectedProject,
                    int.Parse(reader["SocketNum"].ToString())
                    ));
            }

            cn.Close();
        }

        private void loadWLANs()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.WLAN_INFO (@ProjectID)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", selectedProject.ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                currentWLAN = new WirelessLAN(
                    int.Parse(reader["NetResID"].ToString()),
                    selectedProject,
                    reader["SSID"].ToString(),
                    reader["PasswordHash"].ToString()
                    );
                see_more.Visibility = Visibility.Visible;
                wifi_checkbox.IsChecked = true;
                wifi_ssid.Content = "SSID : " + currentWLAN.SSID;
                wifi_password.Password = currentWLAN.PasswordHash;
                wifi_password.IsEnabled = true;
            }

            cn.Close();
        }

        private String launchVM()
        {
            String ip = null;
            int resID = -1;
            OS selectedOS = os_list.SelectedItem as OS;
            String vmPassword = vm_password.Password;

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            // Create new virtual machine
            VirtualMachine vm = new VirtualMachine(
                resID,
                selectedProject,
                VirtualMachine.getIP(),
                vmPassword,
                VirtualMachine.getDockerID(),
                selectedOS
                );

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", vm.ReqProject.ProjectID);
            cmd.Parameters.AddWithValue("@IP", vm.IP);
            cmd.Parameters.AddWithValue("@PasswordHash", vm.PasswordHash);
            cmd.Parameters.AddWithValue("@DockerID", vm.DockerID);
            cmd.Parameters.AddWithValue("@OSID", vm.UsedOS.OSID);
            cmd.Parameters.Add("@resID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.CommandText = "DML.REQUEST_VM";

            try
            {
                cmd.ExecuteNonQuery();
                vm.ResourceID = Convert.ToInt32(cmd.Parameters["@resID"].Value);
                ActiveRequisitionsData.Add(vm);
                os_list.SelectedIndex = 0;
                vm_password.Clear();
                ip = vm.IP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return ip;
        }

        private void saveNetworkChanges()
        {
            // Save possible changes made on WLAN's fields or sockets requisitions
            saveWLAN();
            saveSockets();
        }

        private void saveSockets()
        {
            if (selectedProject == null)
                throw new Exception("You have to select a project first!");

            List<EthernetSocket> toRemove = new List<EthernetSocket>();
            DataTable toRequest = new DataTable();
            toRequest.Clear();
            toRequest.Columns.Add("ResourceID", typeof(int));

            foreach (var resource in socket_list.Items)
            {
                var container = socket_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                if (container == null)
                {
                    socket_list.UpdateLayout();
                    socket_list.ScrollIntoView(resource);
                    container = socket_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                }
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                // Check if the socket is checed for requisition or not
                if (!(((CheckBox)socket_list.ItemTemplate.FindName("active_checkbox", listBoxItemCP)).IsChecked ?? false))
                    continue;

                EthernetSocket socket = resource as EthernetSocket;
                DataRow row = toRequest.NewRow();
                row["ResourceID"] = socket.SocketNum;
                toRequest.Rows.Add(row);
                toRemove.Remove(socket);
            }

            if (toRequest.Rows.Count == 0)
                return;

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            SqlParameter listParam = cmd.Parameters.AddWithValue("@UnitsList", toRequest);
            listParam.SqlDbType = SqlDbType.Structured;
            cmd.Parameters.AddWithValue("@ProjectID", selectedProject.ProjectID);
            cmd.CommandText = "DML.REQUEST_SOCKETS";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();
            
            // Reload sockets and active requisitions lists
            SocketsListData.Clear();
            ActiveRequisitionsData.Clear();
            LoadAvailableSockets();
            LoadProjectActiveRequisitions();
        }

        private void saveWLAN()
        {
            if (currentWLAN == null)
            {
                if (!(wifi_checkbox.IsChecked ?? false))
                    return;

                currentWLAN = new WirelessLAN(
                    -1,
                    selectedProject,
                    "WIFI_" + selectedProject.ProjectID.ToString(),
                    wifi_password.Password
                    );

                createWLAN();
            }
            else if (!(wifi_checkbox.IsChecked ?? false))
                destroyWLAN();
            else
            {
                if (currentWLAN.PasswordHash != wifi_password.Password)
                {
                    currentWLAN.PasswordHash = wifi_password.Password;
                    updateWLAN();
                }
            }
        }

        private void createWLAN()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", selectedProject.ProjectID);
            cmd.Parameters.AddWithValue("@SSID", currentWLAN.SSID);
            cmd.Parameters.AddWithValue("@PasswordHash", currentWLAN.PasswordHash);
            cmd.Parameters.Add("@resID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.CommandText = "DML.REQUEST_WLAN";

            try
            {
                cmd.ExecuteNonQuery();
                currentWLAN.ResourceID = Convert.ToInt32(cmd.Parameters["@resID"].Value);
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


        private void updateWLAN()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            // Update WLAN password
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@resID", currentWLAN.ResourceID);
            cmd.Parameters.AddWithValue("@PasswordHash", currentWLAN.PasswordHash);
            cmd.CommandText = "DML.UPDATE_WLAN";

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

        private void destroyWLAN()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand("DELETE FROM DML.WirelessLAN WHERE NetResID=@resID", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@resID", currentWLAN.ResourceID);

            try
            {
                cmd.ExecuteNonQuery();
                wifi_password.Clear();
                wifi_ssid.Content = "";
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

        private void deliverResources()
        {
            if (selectedProject == null)
                throw new Exception("You have to select a project first!");

            List<EthernetSocket> socketToDeliver = new List<EthernetSocket>();
            List<NetworkResources> toRemove = new List<NetworkResources>();
            DataTable toDeliver = new DataTable();
            toDeliver.Clear();
            toDeliver.Columns.Add("ResourceID", typeof(int));

            foreach (var resource in active_requisitions_list.Items)
            {
                var container = active_requisitions_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;
                
                if (!(((CheckBox)active_requisitions_list.ItemTemplate.FindName("active_checkbox", listBoxItemCP)).IsChecked ?? false))
                    continue;

                NetworkResources unit = resource as NetworkResources;
                if (resource is EthernetSocket)
                    socketToDeliver.Add(resource as EthernetSocket);

                DataRow row = toDeliver.NewRow();
                row["ResourceID"] = unit.ResourceID;
                toDeliver.Rows.Add(row);
                toRemove.Add(unit);
            }

            if (toDeliver.Rows.Count == 0)
                throw new Exception("You can't deliver 0 resources!");

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.CommandText = "DML.DELIVER_NET_RESOURCES";
            SqlParameter listParam = cmd.Parameters.AddWithValue("@UnitsList", toDeliver);
            listParam.SqlDbType = SqlDbType.Structured;

            try
            {
                cmd.ExecuteNonQuery();

                // Clear and reload all lists
                ActiveRequisitionsData.Clear();
                SocketsListData.Clear();
                LoadProjectActiveRequisitions();
                LoadAvailableSockets();
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

        private void request_vm_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check selected project
                checkProject();
                OS selectedOS = os_list.SelectedItem as OS;
                if (selectedOS.OSID == -1)
                    throw new Exception("You need to select an Operating System to the Virtual Machine!");
                String vmPassword = vm_password.Password;
                if (String.IsNullOrEmpty(vmPassword))
                    throw new Exception("Your Virtual Machine needs to be protected by a password!");
                if (vmPassword.Length < 8 || vmPassword.Length > 25)
                    throw new Exception("Your Virtual Machine password needs to have between 8 and 25 characters.");

                MessageBoxResult confirm = MessageBox.Show(
                    "Do you confirm launching a VM running " + selectedOS.OSName + " ?",
                    "VM Launch Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    String ip = launchVM();
                    MessageBox.Show("VM has been launched with success!\nType to access: ssh dmluser@" + ip);
                }
                
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void request_network_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check selected project
                checkProject();
                MessageBoxResult confirm = MessageBox.Show(
                    "Do you confirm these changes?",
                    "Changes Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    saveNetworkChanges();
                    MessageBox.Show("Project's network changes successfully saved!");
                }
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void deliver_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check selected project
                checkProject();
                MessageBoxResult confirm = MessageBox.Show(
                    "Do you to deliver the selected resources?",
                    "Delivery Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    deliverResources();
                    MessageBox.Show("Delivery done with success!");
                }
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void checkProject()
        {
            // Check selected project from the project's list
            foreach (Project resource in projects_list.Items)
            {
                var container = projects_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                RadioButton button = (RadioButton)projects_list.ItemTemplate.FindName("project_button", listBoxItemCP);

                if (button.IsChecked == true)
                    selectedProject = resource;
            }
        }

        private void project_button_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check selected project
                checkProject();

                // Clear all fields and disable buttons
                currentWLAN = null;

                request_vm_button.IsEnabled = true;
                request_network_button.IsEnabled = true;
                deliver_button.IsEnabled = true;

                os_list.IsEnabled = true;

                wifi_checkbox.IsEnabled = true;
                socket_list.IsEnabled = true;

                // Clear active requisitions data and load the active requisitions for selected project
                ActiveRequisitionsData.Clear();
                wifi_checkbox.IsChecked = false;
                see_more.Visibility = Visibility.Hidden;
                wifi_password.Password = "";
                wifi_ssid.Content = "";
                LoadProjectActiveRequisitions();
                loadWLANs();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged_Sockets(object sender, TextChangedEventArgs e)
        {
            // Filter sockets which contains writed keyword
            if (SocketsListData.Count > 0 && !search_box_sockets.Text.Equals(""))
            {
                var filteredSockets = SocketsListData.Where(i => ((EthernetSocket)i).ToString().ToLower().Contains(search_box_sockets.Text.ToLower())).ToArray();
                socket_list.ItemsSource = filteredSockets;
            }
            else
            {
                socket_list.ItemsSource = SocketsListData;
            }
        }

        private void TextBox_TextChanged_Requisitions(object sender, TextChangedEventArgs e)
        {
            // Filter requisitions which contains writed keyword
            if (ActiveRequisitionsData.Count > 0 && !search_box_requisitions.Text.Equals(""))
            {
                var filteredRequisitions = ActiveRequisitionsData.Where(i => ((NetworkResources)i).ToString().ToLower().Contains(search_box_requisitions.Text.ToLower())).ToArray();
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

        private void wifi_checkbox_Click(object sender, RoutedEventArgs e)
        {
            // Change wifi password status
            wifi_password.IsEnabled = wifi_checkbox.IsChecked ?? false;
        }

        private void os_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Change vm password status
            vm_password.IsEnabled = os_list.SelectedIndex != 0;
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // See, while clicking, the password in plain text
            wifi_password_clear.Text = wifi_password.Password;
            wifi_password.Visibility = System.Windows.Visibility.Hidden;
            wifi_password_clear.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Change the password to password's view
            wifi_password.Visibility = System.Windows.Visibility.Visible;
            wifi_password_clear.Visibility = System.Windows.Visibility.Hidden;
        }

        private void vm_password_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                request_vm_button_Click(this, new RoutedEventArgs());
            }
        }

        private void wifi_password_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                request_network_button_Click(this, new RoutedEventArgs());
            }
        }
    }
}
