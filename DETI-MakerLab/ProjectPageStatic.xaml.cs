using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ProjectPageStatic.xaml
    /// </summary>
    public partial class ProjectPageStatic : Page
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Requisition> RequisitionsData;
        private Project _project;

        public ProjectPageStatic(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RequisitionsData = new ObservableCollection<Requisition>();
            loadUsers();
            loadRequisitions();
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            project_members.ItemsSource = MembersListData;
            project_last_requisitions_list.ItemsSource = RequisitionsData;
            project_members.MouseDoubleClick += new MouseButtonEventHandler(project_members_listbox_MouseDoubleClick);
        }

        private void loadUsers()
        {
            foreach (DMLUser worker in _project.Workers)
                MembersListData.Add(worker);
        }

        private Requisition getRequisition(Requisition req)
        {
            foreach (Requisition r in RequisitionsData)
                if (r.Equals(req))
                    return r;
            return null;
        }

        private void loadRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@pID", _project.ProjectID);
            cmd.CommandText = "SELECT * FROM PROJECT_REQS (@pID)";
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Class cl = null;
                if (reader["ClassID"] != DBNull.Value)
                    cl = new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    );

                RequisitionsData.Add(new Requisition(
                        int.Parse(reader["RequisitionID"].ToString()),
                        new Project(
                            int.Parse(reader["ProjectID"].ToString()),
                            reader["PrjName"].ToString(),
                            reader["PrjDescription"].ToString(),
                            cl),
                        new DMLUser(
                            int.Parse(reader["NumMec"].ToString()),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["Email"].ToString(),
                            reader["PathToImage"].ToString()
                            ),
                        Convert.ToDateTime(reader["ReqDate"])
                    ));

            }
            cn.Close();
        }

        private void project_members_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (project_members.SelectedItem != null)
            {
                DMLUser user = project_members.SelectedItem as DMLUser;
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goToUserPage(user);
            }
        }

        private void go_back_button_Click(object sender, RoutedEventArgs e)
        {
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goBack();
        }
    }
}
