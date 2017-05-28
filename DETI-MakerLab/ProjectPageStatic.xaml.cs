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
    public partial class ProjectPageStatic : Page, DMLPages
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Requisition> RequisitionsData;
        private List<Role> Roles;
        private Project _project;

        public ProjectPageStatic(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RequisitionsData = new ObservableCollection<Requisition>();
            Roles = new List<Role>();
            try
            {
                LoadRoles();
                loadUsers();
                loadRequisitions();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            project_members.ItemsSource = MembersListData;
            project_last_requisitions_list.ItemsSource = RequisitionsData;
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
            try
            {
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goBack();
            } catch (Exception ex)
            {
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goBack();
            }
        }

        private void project_last_requisitions_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (project_last_requisitions_list.SelectedItem != null)
            {
                Requisition requisition = project_last_requisitions_list.SelectedItem as Requisition;
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
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
