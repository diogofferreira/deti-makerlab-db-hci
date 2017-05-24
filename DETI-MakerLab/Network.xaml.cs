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
    public partial class Network : Page
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
            ProjectsListData = new ObservableCollection<Project>();
            SocketsListData = new ObservableCollection<EthernetSocket>();
            ActiveRequisitionsData = new ObservableCollection<NetworkResources>();
            OSList = new ObservableCollection<OS>();
            try
            {
                LoadOS();
                LoadProjects(UserID);
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            projects_list.ItemsSource = ProjectsListData;
            socket_list.ItemsSource = SocketsListData;
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
        }

        private void LoadProjects(int UserID)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

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

        private void LoadOS()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM OS", cn);
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

            List<int> availableSockets = Enumerable.Range(1, 20).ToList();
            SqlCommand cmd = new SqlCommand("SELECT * FROM AVAILABLE_SOCKETS()", cn);
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

            loadVMs();
            loadSockets();
            loadWLANs();
        }

        private void loadVMs()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM VM_INFO (@ProjectID)", cn);
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

            SqlCommand cmd = new SqlCommand("SELECT * FROM SOCKETS_INFO (@ProjectID)", cn);
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

            SqlCommand cmd = new SqlCommand("SELECT * FROM WLAN_INFO (@ProjectID)", cn);
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
                wifi_checkbox.IsChecked = true;
                wifi_password.Password = currentWLAN.PasswordHash;
            }

            cn.Close();
        }

        // Falta a saveChanges()!!!!!!!!
        private void launchVM()
        {
            int resID = -1;
            OS selectedOS = os_list.SelectedItem as OS;
            if (selectedOS.OSID == -1)
                throw new Exception("You need to select an Operating System to the Virtual Machine!");
            String vmPassword = vm_password.Password;
            if (String.IsNullOrEmpty(vmPassword))
                throw new Exception("Your Virtual Machine needs to be protected by a password!");
            if (vmPassword.Length < 8 || vmPassword.Length > 25)
                throw new Exception("Your Virtual Machine password needs to have between 8 and 25 characters.");

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
            cmd.Parameters.AddWithValue("@DockerID", vm.UsedOS.OSID);
            cmd.Parameters.Add("@resID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.CommandText = "dbo.REQUEST_VM";

            try
            {
                cmd.ExecuteNonQuery();
                vm.ResourceID = Convert.ToInt32(cmd.Parameters["resID"].Value);
                ActiveRequisitionsData.Add(vm);
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

        private void saveNetworkChanges()
        {
            saveWLAN();
            saveSockets();
        }

        private void saveSockets()
        {
            if (selectedProject == null)
                throw new Exception("You have to select a project first!");

            DataTable toRequest = new DataTable();
            toRequest.Clear();
            toRequest.Columns.Add("ResourceID", typeof(int));

            foreach (var resource in socket_list.Items)
            {
                var container = socket_list.ItemContainerGenerator.ContainerFromItem(resource) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                if (!((CheckBox)socket_list.ItemTemplate.FindName("active_checkbox", listBoxItemCP)).IsChecked ?? false)
                    continue;

                EthernetSocket socket = resource as EthernetSocket;
                DataRow row = toRequest.NewRow();
                row["ResourceID"] = socket.ResourceID;
            }

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
            cmd.CommandText = "dbo.REQUEST_SOCKETS";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ActiveRequisitionsData.Add(new EthernetSocket(
                    int.Parse(row["ResourceID"].ToString()),
                    selectedProject,
                    int.Parse(row["SocketNum"].ToString())
                    ));
            }
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
                // CHECK IF THEY ARE DIFFERENT FIRST
                currentWLAN.PasswordHash = wifi_password.Password;
                updateWLAN();
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
            cmd.CommandText = "dbo.REQUEST_WLAN";

            try
            {
                cmd.ExecuteNonQuery();
                currentWLAN.ResourceID = Convert.ToInt32(cmd.Parameters["resID"].Value);
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


        private void updateWLAN()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand("UPDATE WirelessLAN SET PasswordHash = @PasswordHash WHERE NetResID=@resID)", cn);
            cmd.Parameters.AddWithValue("@PasswordHash", currentWLAN.PasswordHash);
            cmd.Parameters.AddWithValue("@resID", currentWLAN.ResourceID);

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

        private void destroyWLAN()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand("DELETE FROM WirelessLAN WHERE NetResID=@resID)", cn);
            cmd.Parameters.AddWithValue("@resID", currentWLAN.ResourceID);

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

        private void deliverResources()
        {
            if (selectedProject == null)
                throw new Exception("You have to select a project first!");

            List<EthernetSocket> socketToDeliver = new List<EthernetSocket>();
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

                if (!((CheckBox)active_requisitions_list.ItemTemplate.FindName("active_checkbox", listBoxItemCP)).IsChecked ?? false)
                    continue;

                NetworkResources unit = resource as NetworkResources;
                if (resource is EthernetSocket)
                    socketToDeliver.Add(resource as EthernetSocket);

                DataRow row = toDeliver.NewRow();
                row["ResourceID"] = unit.ResourceID;
            }

            if (toDeliver.Rows.Count == 0)
                throw new Exception("You can't deliver 0 resources!");

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.CommandText = "dbo.DELIVER_NET_RESOURCES";
            SqlParameter listParam = cmd.Parameters.AddWithValue("@UnitsList", toDeliver);
            listParam.SqlDbType = SqlDbType.Structured;

            try
            {
                cmd.ExecuteNonQuery();
                foreach (EthernetSocket socket in socketToDeliver) {
                    socket.ResourceID = -1;
                    socket.ReqProject = null;
                    SocketsListData.Add(socket);
                }
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

        private void request_vm_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkProject();
                launchVM();
                MessageBox.Show("VM has been launched with success!");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            // TODO : show a message with more content, like ssh command
        }

        private void request_network_button_Click(object sender, RoutedEventArgs e)
        {
            saveNetworkChanges();
            MessageBox.Show("Network's requisition done with success !");
        }

        private void deliver_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delivery done with success !");
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
                    selectedProject = resource;
            }
        }

        private void project_button_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                checkProject();
                LoadProjectActiveRequisitions();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
