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
    public partial class MyProjects : Page
    {
        private ObservableCollection<Project> ProjectsListData;
        private SqlConnection cn;

        public MyProjects(int userID)
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            LoadProjects(userID);
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
                Project prj = new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    ));
                ProjectsListData.Add(prj);
            }

            cn.Close();
        }
    }
}
