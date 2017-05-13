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
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectChanges : Page
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private Project _project;
        private List<Role> _roles = new List<Role>();

        internal List<Role> Roles
        {
            get { return _roles; }
        }

        public ProjectChanges(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();

            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            // Hardcoded Data
            MembersListData.Add(new Student(78452, "Rui", "Lemos", "ruilemos@ua.pt", "hash", "none", "ECT"));
            project_members.ItemsSource = MembersListData;
        }

        private void LoadRoles()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            project_members.Items.Clear();

            while (reader.Read())
            {
                Role R = new Role(int.Parse(reader["RoleID"].ToString()), reader["RoleDescription"].ToString());
                Roles.Add(R);
            }

            cn.Close();
        }

        private void LoadMembers()
        { 

            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM DMLUser", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<DMLUser> tmp = new List<DMLUser>();

            while (reader.Read())
            {
                bool team = false;
                DMLUser User = new DMLUser();
                User.NumMec = int.Parse(reader["NumMec"].ToString());
                User.FirstName = reader["FirstName"].ToString();
                User.LastName = reader["LastName"].ToString();
                User.Email = reader["Email"].ToString();
                User.PasswordHash = reader["PasswordHash"].ToString();
                User.PathToImage = reader["PathToImage"].ToString();

                foreach (int[] tuple in _project.Workers)
                {
                    if (tuple[0] == User.NumMec)
                    {
                        team = true;
                        MembersListData.Add(User);
                        // É preciso editar o Role, não sei como fazer isso
                    }

                }
                if (!team)
                    tmp.Add(User);
            }

            foreach (DMLUser User in tmp)
                MembersListData.Add(User);

            cn.Close();
        }

        private void saveChanges()
        {
            _project.ProjectDescription = project_description.Text;
            // Save project members
        }

        private void save_project_changes_button_Click(object sender, RoutedEventArgs e)
        {
            saveChanges();
            MessageBox.Show("The project has been changed!");
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goToProjectPage(_project);
        }

        private SqlConnection getSGBDConnection()
        {
            return new SqlConnection("data source= DESKTOP-H41EV9L\\SQLEXPRESS;integrated security=true;initial catalog=Northwind");
        }

        private bool verifySGBDConnection()
        {
            if (cn == null)
                cn = getSGBDConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }
    }
}
