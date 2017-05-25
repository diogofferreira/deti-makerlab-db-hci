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
    /// Interaction logic for CreateProject.xaml
    /// </summary>
    public partial class CreateProject : Page
    {
        SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Class> ClassListData;
        private static ObservableCollection<Role> RolesListData;

        public CreateProject()
        {
            InitializeComponent();
            MembersListData = new ObservableCollection<DMLUser>();
            RolesListData = new ObservableCollection<Role>();
            ClassListData = new ObservableCollection<Class>();
            LoadRoles();
            LoadClasses();
            LoadMembers();
            project_members.ItemsSource = MembersListData;
            project_class.ItemsSource = ClassListData;
        }

        public ObservableCollection<Role> RolesList 
        {
            get {
                return RolesListData;
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
                Console.WriteLine(((Class)project_class.SelectedValue).ClassID);
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
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
            }
            finally
            {
                cn.Close();
            }

            return projectID;
        }

        private void SubmitMembers(int projectID)
        {
            SqlCommand cmd;
            
            foreach (DMLUser checkedMember in project_members.Items)
            {
                var container = project_members.ItemContainerGenerator.ContainerFromItem(checkedMember) as FrameworkElement;
                ContentPresenter listBoxItemCP = Helpers.FindVisualChild<ContentPresenter>(container);
                if (listBoxItemCP == null)
                    return;

                DataTemplate dataTemplate = listBoxItemCP.ContentTemplate;

                Role r = (Role)((ComboBox)project_members.ItemTemplate.FindName("member_role", listBoxItemCP)).SelectedItem;
                if (r.RoleID == -1)
                    continue;

                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("INSERT INTO WorksOn (UserNMec, ProjectID, UserRole) " +
                    "VALUES (@UserNMec, @ProjectID, @UserRole)", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", checkedMember.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                cmd.Parameters.AddWithValue("@UserRole", r.RoleID);

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
            
            
        }

        private void create_project_button_Click(object sender, RoutedEventArgs e)
        {
            try { 
                int projectID = SubmitProject();
                if (projectID != -1)
                    SubmitMembers(projectID);
                MessageBox.Show("Project has been created !");
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                // TODO : create project object
                //window.goToProjectPage(_project);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            
        }
    }
}
