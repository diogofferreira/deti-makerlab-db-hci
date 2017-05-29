using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page, DMLPages
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Resources> ActiveRequisitionsData;
        private ObservableCollection<Requisition> RequisitionsData;
        private List<Role> Roles;
        private Project _project;

        public ProjectPage(Project project, bool created = false)
        {
            InitializeComponent();
            this._project = project;
            // Hide go back button if the project has been recently created
            if (created)
                go_back_button.Visibility = Visibility.Hidden;
            MembersListData = new ObservableCollection<DMLUser>();
            ActiveRequisitionsData = new ObservableCollection<Resources>();
            RequisitionsData = new ObservableCollection<Requisition>();
            Roles = new List<Role>();
            try
            {
                // Load project members (and it's roles), last requisitions and active requisitions
                LoadRoles();
                loadUsers();
                loadRequisitions();
                LoadProjectActiveRequisitons();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Set project member
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            project_class.Text = _project.ProjectClass == null ? "Standalone Project" : _project.ProjectClass.ClassName;
            project_members.ItemsSource = MembersListData;
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
            project_last_requisitions_list.ItemsSource = RequisitionsData;
            // Set member's listbox listener
            project_members.MouseDoubleClick += new MouseButtonEventHandler(project_members_listbox_MouseDoubleClick);
        }

        private void LoadRoles()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            Roles.Add(new Role(-1, "Not a Member"));

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Roles.Add(new Role(
                    int.Parse(reader["RoleID"].ToString()),
                    reader["RoleDescription"].ToString())
                    );
            }

            cn.Close();
        }

        private String getRoleDescription(int roleID)
        {
            // Get role nome by it's id
            foreach (Role r in Roles)
                if (r.RoleID == roleID)
                    return r.RoleDescription;
            return "Not a member";
        }

        private void loadUsers()
        {
            foreach (DMLUser worker in _project.Workers)
            {
                worker.RoleDescription = getRoleDescription(worker.RoleID);
                MembersListData.Add(worker);
            }
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

        private void LoadProjectActiveRequisitons()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@pID", _project.ProjectID);
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

        private void project_members_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // Go to selected member's page
            if (project_members.SelectedItem != null)
            {
                DMLUser user = project_members.SelectedItem as DMLUser;
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToUserPage(user);
            }
        }

        private void manage_project_button_Click(object sender, RoutedEventArgs e)
        {
            // Go to change project page
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goToChangeProjectPage(_project);
        }

        private void go_back_button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to last page
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goBack();
        }

        private void project_last_requisitions_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Go to selected requisition page
            if (project_last_requisitions_list.SelectedItem != null)
            {
                Requisition requisition = project_last_requisitions_list.SelectedItem as Requisition;
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToRequisitionPage(requisition);
            }
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
