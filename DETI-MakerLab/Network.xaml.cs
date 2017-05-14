using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int _userID;

        public Network(int UserID)
        {
            InitializeComponent();
            this._userID = UserID;
            ProjectsListData = new ObservableCollection<Project>();
            SocketsListData = new ObservableCollection<EthernetSocket>();
            ActiveRequisitionsData = new ObservableCollection<NetworkResources>();
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
