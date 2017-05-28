using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for CreateProject.xaml
    /// </summary>
    public partial class CreateProject : Page, DMLPages
    {
        SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Class> ClassListData;
        private static ObservableCollection<Role> RolesListData;
        private int _userID;
        private Project _project;

        public CreateProject(int userID)
        {
            InitializeComponent();
            this._userID = userID;
            MembersListData = new ObservableCollection<DMLUser>();
            RolesListData = new ObservableCollection<Role>();
            ClassListData = new ObservableCollection<Class>();
            try
            {
                LoadRoles();
                LoadClasses();
                LoadMembers();
            } catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            } catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            project_members.ItemsSource = MembersListData;
            project_class.ItemsSource = ClassListData;
            project_members.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        public ObservableCollection<Role> RolesList 
        {
            get { return RolesListData; }
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (project_members.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                SetMyRole();
        }

        private void SetMyRole()
        {
            foreach (DMLUser member in project_members.Items)
            {
                if (member.NumMec != _userID)
                    continue;
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
                ((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedIndex = 5;
            }
        }

        private void LoadRoles()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            RolesListData.Add(new Role(-1, "Not a Member"));

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            project_members.Items.Clear();

            while (reader.Read())
            {
                RolesListData.Add(new Role(
                    int.Parse(reader["RoleID"].ToString()),
                    reader["RoleDescription"].ToString())
                    );
            }

            cn.Close();
        }

        private void LoadClasses()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            ClassListData.Add(new Class(-1, "No Class", "Standalone Project"));

            SqlCommand cmd = new SqlCommand("SELECT * FROM Class", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            project_members.Items.Clear();

            while (reader.Read())
            {
                ClassListData.Add(new Class(
                    int.Parse(reader["ClassID"].ToString()),
                    reader["ClassName"].ToString(),
                    reader["ClDescription"].ToString()
                    ));
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

            while (reader.Read())
            {
                DMLUser User = new DMLUser();
                User.NumMec = int.Parse(reader["NumMec"].ToString());
                User.FirstName = reader["FirstName"].ToString();
                User.LastName = reader["LastName"].ToString();
                User.Email = reader["Email"].ToString();
                User.PathToImage = reader["PathToImage"].ToString();

                MembersListData.Add(User);
            }

            cn.Close();
        }

        private int SubmitProject()
        {
            SqlCommand cmd;
            int projectID = -1;
            
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PrjName", project_name.Text);
            cmd.Parameters.AddWithValue("@PrjDescription", project_description.Text);
            cmd.Parameters.Add("@ProjectID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.CommandText = "dbo.CREATE_PROJECT";

            if (((Class)project_class.SelectedValue).ClassID != -1)
            {
                cmd.Parameters.AddWithValue("@ClassID", ((Class)project_class.SelectedValue).ClassID);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ClassID", DBNull.Value);
            }

            try
            {
                cmd.ExecuteNonQuery();
                projectID = Convert.ToInt32(cmd.Parameters["@ProjectID"].Value);
                _project = new Project(
                    projectID,
                    project_name.Text,
                    project_description.Text
                    );
                if (((Class)project_class.SelectedValue).ClassID != -1)
                    _project.ProjectClass = ((Class)project_class.SelectedValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }

            return projectID;
        }

        private void SubmitMembers(int projectID)
        {
            DataTable members = new DataTable();
            members.Clear();
            members.Columns.Add("UserID", typeof(decimal));
            members.Columns.Add("RoleID", typeof(int));

            foreach (DMLUser checkedMember in project_members.Items)
            {
                var container = project_members.ItemContainerGenerator.ContainerFromItem(checkedMember) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                Role r = (Role)((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedItem;
                if (checkedMember.NumMec == _userID && r.RoleID == -1)
                    throw new Exception("You must belong to a project you create!");

                if (r == null || r.RoleID == -1)
                    continue;

                DataRow row = members.NewRow();
                row["UserID"] = checkedMember.NumMec;
                row["RoleID"] = r.RoleID;
                members.Rows.Add(row);
                checkedMember.RoleID = r.RoleID;
                _project.addWorker(checkedMember);
            }

            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("ADD_PROJECT_USERS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@projectID", projectID);
            SqlParameter listParam = cmd.Parameters.AddWithValue("@WorkersList", members);
            listParam.SqlDbType = SqlDbType.Structured;

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

        private void checkMandatoryFields()
        {
            if (String.IsNullOrEmpty(project_name.Text) || String.IsNullOrEmpty(project_description.Text)
                || project_class.SelectedIndex < 0)
                throw new Exception("Please fill the mandatory fields!");
            foreach (DMLUser checkedMember in project_members.Items)
            {
                var container = project_members.ItemContainerGenerator.ContainerFromItem(checkedMember) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                Role r = (Role)((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedItem;
                if (checkedMember.NumMec == _userID && r.RoleID == -1)
                    throw new Exception("You must belong to a project you create!");
            }
        }

        public bool isEmpty()
        {
            if (!String.IsNullOrEmpty(project_name.Text) || !String.IsNullOrEmpty(project_description.Text)
                || !(project_class.SelectedIndex < 0))
                return false;
            return true;
        }

        private void create_project_button_Click(object sender, RoutedEventArgs e)
        {
            try {
                checkMandatoryFields();
                MessageBoxResult confirm = MessageBox.Show(
                    "Do you want to submit this project?", 
                    "Submission Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    int projectID = SubmitProject();
                    if (projectID != -1)
                        SubmitMembers(projectID);
                    MessageBox.Show("Project has been created!");
                    HomeWindow window = (HomeWindow)Window.GetWindow(this);
                    window.goToProjectPage(_project);
                }
            } catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filter users which contains writed keyword
            if (MembersListData.Count > 0 && !search_box.Text.Equals(""))
            {
                var filteredUsers = MembersListData.Where(i => ((DMLUser)i).FullName.ToLower().Contains(search_box.Text.ToLower())).ToArray();
                project_members.ItemsSource = filteredUsers;
            } else
            {
                project_members.ItemsSource = MembersListData;
            }
        }
    }
}
