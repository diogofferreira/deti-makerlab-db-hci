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
using System.Windows.Controls.Primitives;
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
    public partial class ProjectChanges : Page, DMLPages
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Role> RolesListData;
        private Project _project;

        public ProjectChanges(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RolesListData = new ObservableCollection<Role>();
            try
            {
                LoadRoles();
                LoadMembers();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error
                    );
            }
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            project_class.Text = _project.ProjectClass.ClassName;
            project_members.ItemsSource = MembersListData;
            project_members.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        private void LoadRoles()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            RolesListData.Add(new Role(-1, "Not a Member"));

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                RolesListData.Add(new Role(
                    int.Parse(reader["RoleID"].ToString()),
                    reader["RoleDescription"].ToString())
                    );
            }

            cn.Close();
        }

        private void LoadMembers()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DMLUser", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            
            foreach (DMLUser worker in _project.Workers)
                MembersListData.Add(worker);

            while (reader.Read())
            {
                DMLUser User = new DMLUser();
                User.NumMec = int.Parse(reader["NumMec"].ToString());
                User.FirstName = reader["FirstName"].ToString();
                User.LastName = reader["LastName"].ToString();
                User.Email = reader["Email"].ToString();
                User.PathToImage = reader["PathToImage"].ToString();
                User.RoleID = -1;

                if (!_project.hasWorker(User))
                    MembersListData.Add(User);
            }

            cn.Close();
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (project_members.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                SetRoles();
            }
        }

        private void SetRoles()
        {
            foreach (DMLUser member in project_members.Items)
            {
                var container = project_members.ItemContainerGenerator.ContainerFromItem(member) as FrameworkElement;
                if (container == null)
                {
                    project_members.UpdateLayout();
                    project_members.ScrollIntoView(member);
                    container = project_members.ItemContainerGenerator.ContainerFromItem(member) as FrameworkElement;
                }
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                ((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedIndex = findIndexRole(member.RoleID);
            }
        }

        private int findIndexRole(int id)
        {
            for (int i = 0; i < RolesListData.Count; i++)
            {
                if (RolesListData[i].RoleID == id)
                    return i;
            }
            return 0;
        }

        private void saveChanges()
        {
            _project.ProjectDescription = project_description.Text;
            // Save project members
            List<DMLUser> newWorker = new List<DMLUser>();
            List<DMLUser> updateWorker = new List<DMLUser>();
            List<DMLUser> removeWorker = new List<DMLUser>();


            foreach (DMLUser member in project_members.Items)
            {
                var container = project_members.ItemContainerGenerator.ContainerFromItem(member) as FrameworkElement;
                if (container == null)
                {
                    project_members.UpdateLayout();
                    project_members.ScrollIntoView(member);
                    container = project_members.ItemContainerGenerator.ContainerFromItem(member) as FrameworkElement;
                }
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                // Set me as Project Manager by default
                Role r = ((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedItem as Role;
                //member.RoleID = r.RoleID;

                Console.WriteLine(member);
                Console.WriteLine(_project.workerChanges(member.NumMec, r.RoleID));

                if (r.RoleID != -1)
                {
                    switch (_project.workerChanges(member.NumMec, r.RoleID))
                    {
                        case 1:
                            updateWorker.Add(member);
                            break;

                        case 2:
                            newWorker.Add(member);
                            break;

                        default:
                            break;
                    }
                }
                else if (_project.workerChanges(member.NumMec, r.RoleID) == 1)
                    removeWorker.Add(member);
                    

                member.RoleID = r.RoleID;
            }

            addWorkers(newWorker);
            updateWorkers(updateWorker);
            removeWorkers(removeWorker);
        }

        private void addWorkers(List<DMLUser> newWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in newWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("INSERT INTO WorksOn (UserNMec, ProjectID, UserRole) " +
                    "VALUES (@UserNMec, @ProjectID, @UserRole)", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);
                cmd.Parameters.AddWithValue("@UserRole", user.RoleID);

                try
                {
                    cmd.ExecuteNonQuery();
                    _project.addWorker(user);
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
        }

        private void updateWorkers(List<DMLUser> updateWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in updateWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("UPDATE WorksOn SET UserRole=@UserRole " +
                    "WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);
                cmd.Parameters.AddWithValue("@UserRole", user.RoleID);

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
        }

        private void removeWorkers(List<DMLUser> removeWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in removeWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("DELETE FROM WorksOn " +
                    "WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);

                try
                {
                    cmd.ExecuteNonQuery();
                    _project.removeWorker(user);
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
        }

        private void checkMandatoryFields()
        {
            if (String.IsNullOrEmpty(project_name.Text) || String.IsNullOrEmpty(project_description.Text))
                throw new Exception("Please fill the mandatory fields!");
        }

        public bool isEmpty()
        {
            // Check if there are unsaved fields
            if (!String.IsNullOrEmpty(project_name.Text) || !String.IsNullOrEmpty(project_description.Text))
                return false;
            return true;
        }

        private void save_project_changes_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkMandatoryFields();
                MessageBoxResult confirm = MessageBox.Show(
                    "Do you confirm these changes?",
                    "Changes Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    saveChanges();
                    MessageBox.Show("The project has been changed!");
                    HomeWindow window = (HomeWindow)Window.GetWindow(this);
                    window.goToProjectPage(_project);
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
    }
}
