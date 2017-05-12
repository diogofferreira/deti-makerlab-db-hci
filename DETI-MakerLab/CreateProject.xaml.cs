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
        private List<Role> _roles = new List<Role>();

        internal List<Role> Roles
        {
            get { return _roles; }
        }

        public CreateProject()
        {
            InitializeComponent();
            MembersListData = new ObservableCollection<DMLUser>();
            //LoadMembers();
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
            project_members.Items.Clear();

            while (reader.Read())
            {
                DMLUser User = new DMLUser();
                User.NumMec = int.Parse(reader["NumMec"].ToString());
                User.FirstName = reader["FirstName"].ToString();
                User.LastName = reader["LastName"].ToString();
                User.Email = reader["Email"].ToString();
                User.PasswordHash = reader["PasswordHash"].ToString();
                User.PathToImage = reader["PathToImage"].ToString();

                project_members.Items.Add(User);
            }

            cn.Close();
        }

        private void SubmitProject()
        {
            SqlCommand cmd;
            int returnedID;
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            cmd = new SqlCommand("INSERT INTO Project (PrjName, PrjDescription, Class) " +
                "OUTPUT Inserted.ProjectID VALUES (@PrjName, @PrjDescription, @Class)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PrjName", project_name.Text);
            cmd.Parameters.AddWithValue("@PrjDescription", project_description.Text);
            if (String.IsNullOrEmpty(project_class.Text))
            {
                cmd.Parameters.AddWithValue("@Class", "NULL");
            }
            else
            {
                cmd.Parameters.AddWithValue("@Class", project_class.Text);
            }

            try
            {
                returnedID = (int)cmd.ExecuteScalar();
                SubmitMembers(returnedID);
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

        private void SubmitMembers(int projectID)
        {
            SqlCommand cmd;

            /*
            foreach (DMLUser checkedMember in project_members.Items)
            {
                cn = getSGBDConnection();
                if (!verifySGBDConnection())
                    return;

                cmd = new SqlCommand("INSERT INTO WorksOn (UserNMec, ProjectID, UserRole) " +
                    "VALUES (@UserNMec, @ProjectID, @UserRole)", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", checkMember.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                cmd.Parameters.AddWithValue("@UserRole", checkMember.Role.RoleID);

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
            */
            
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
