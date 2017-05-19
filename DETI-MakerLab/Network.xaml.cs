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

        private int _userID;

        public Network(int UserID)
        {
            InitializeComponent();
            this._userID = UserID;
            ProjectsListData = new ObservableCollection<Project>();
            SocketsListData = new ObservableCollection<EthernetSocket>();
            ActiveRequisitionsData = new ObservableCollection<NetworkResources>();
            OSList = new ObservableCollection<OS>();
            /*
            LoadOS();
            LoadProjects();
            LoadActiveRequisitions();
             * */

            // Hardcoded Data
            Project proj = new Project(1, "DETI-MakerLab", "DETI-MakerLab Project Description");
            ProjectsListData.Add(proj);
            ProjectsListData.Add(new Project(2, "BlueConf", "BlueConf Project Description"));
            projects_list.ItemsSource = ProjectsListData;
            SocketsListData.Add(new EthernetSocket(3, proj, 0));
            SocketsListData.Add(new EthernetSocket(4, proj, 1));
            socket_list.ItemsSource = SocketsListData;
            ActiveRequisitionsData.Add(new NetworkResources(4, proj));
            ActiveRequisitionsData.Add(new NetworkResources(5, proj));
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
        }

        private void LoadProjects()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM Project", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ProjectsListData.Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()), 
                    reader["PrjName"].ToString(), 
                    reader["PrjDescription"].ToString()));
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

        private void LoadActiveRequisitions()
        {
            if (projects_list.SelectedItem == null)
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

            SqlCommand cmd = new SqlCommand("SELECT * FROM VM_INFO WHERE ReqProject=@ProjectID", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", ((Project)projects_list.SelectedItem).ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ActiveRequisitionsData.Add(new VirtualMachine(
                    int.Parse(reader["NetResID"].ToString()),
                    (Project)projects_list.SelectedItem,
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

            SqlCommand cmd = new SqlCommand("SELECT * FROM SOCKET_INFO WHERE ReqProject=@ProjectID", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", ((Project)projects_list.SelectedItem).ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ActiveRequisitionsData.Add(new EthernetSocket(
                    int.Parse(reader["NetResID"].ToString()),
                    (Project)projects_list.SelectedItem,
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

            SqlCommand cmd = new SqlCommand("SELECT * FROM WLAN_INFO WHERE ReqProject=@ProjectID", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", ((Project)projects_list.SelectedItem).ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ActiveRequisitionsData.Add(new WirelessLAN(
                    int.Parse(reader["NetResID"].ToString()),
                    (Project)projects_list.SelectedItem,
                    reader["SSID"].ToString(),
                    reader["PasswordHash"].ToString()
                    ));
            }

            cn.Close();
        }

        // Falta a saveChanges()!!!!!!!!

        private void request_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Request done with sucess !");
        }

        private void deliver_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delivery done with sucess !");
        }
    }
}
