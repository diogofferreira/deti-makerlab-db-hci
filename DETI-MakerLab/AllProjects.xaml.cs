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
    /// Interaction logic for AllProjects.xaml
    /// </summary>
    public partial class AllProjects : Page
    {
        SqlConnection cn;
        private ObservableCollection<Project> ProjectsListData;

        public AllProjects()
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            LoadProjects();
            LoadUsers();
            all_projects_listbox.ItemsSource = ProjectsListData;
        }

        private void all_projects_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (all_projects_listbox.SelectedItem != null)
            {
                Project selectedProject = all_projects_listbox.SelectedItem as Project;
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goToProjectPage(selectedProject);
            }
        }

        private void LoadProjects()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM PROJECT_INFO", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            all_projects_listbox.Items.Clear();

            while (reader.Read())
            {
                Class cl = null;
                if (reader["ClassID"] != DBNull.Value)
                    cl = new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    );

                ProjectsListData.Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    cl
                    ));
            }

            cn.Close();
        }

        private void LoadUsers()
        {
            foreach (Project proj in ProjectsListData)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    return;

                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("PROJECT_USERS", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@pID", proj.ProjectID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Student s = new Student(
                            int.Parse(row["NumMec"].ToString()),
                            row["FirstName"].ToString(),
                            row["LastName"].ToString(),
                            row["Email"].ToString(),
                            row["PathToImage"].ToString(),
                            row["Course"].ToString()
                        );
                    s.RoleID = int.Parse(row["UserRole"].ToString());
                    proj.addWorker(s);
                }

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    Professor p = new Professor(
                            int.Parse(row["NumMec"].ToString()),
                            row["FirstName"].ToString(),
                            row["LastName"].ToString(),
                            row["Email"].ToString(),
                            row["PathToImage"].ToString(),
                            row["ScientificArea"].ToString()
                        );
                    p.RoleID = int.Parse(row["UserRole"].ToString());
                    proj.addWorker(p);
                }
            }
        }
    }
}
