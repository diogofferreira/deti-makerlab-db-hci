using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for MyProjects.xaml
    /// </summary>
    public partial class MyProjects : Page, DMLPages
    {
        private ObservableCollection<Project> ProjectsListData;
        private SqlConnection cn;

        public MyProjects(int userID)
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            try
            {
                LoadProjects(userID);
                LoadUsers();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            my_projects_listbox.ItemsSource = ProjectsListData;
            my_projects_listbox.MouseDoubleClick += new MouseButtonEventHandler(my_projects_listbox_MouseDoubleClick);
        }

        private void my_projects_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (my_projects_listbox.SelectedItem != null)
            {
                Project selectedProject = my_projects_listbox.SelectedItem as Project;
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToProjectPage(selectedProject);
            }
        }

        private void LoadProjects(int userID)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM USER_PROJECTS (@nmec)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@nmec", userID);
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

        private void TextBox_TextChanged_Projects(object sender, TextChangedEventArgs e)
        {
            // Filter projects which contains writed keyword
            if (ProjectsListData.Count > 0 && !search_box_projects.Text.Equals(""))
            {
                var filteredProjects = ProjectsListData.Where(i => ((Project)i).ProjectName.ToLower().Contains(search_box_projects.Text.ToLower())).ToArray();
                my_projects_listbox.ItemsSource = filteredProjects;
            }
            else
            {
                my_projects_listbox.ItemsSource = ProjectsListData;
            }
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
