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
            ProjectsListData.Add(new Project(1, "DETI-MakerLab", "DETI-MakerLab Project Description"));
            ProjectsListData.Add(new Project(2, "BlueConf", "BlueConf Project Description"));
            all_projects_listbox.ItemsSource = ProjectsListData;
            //LoadProjects();
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
