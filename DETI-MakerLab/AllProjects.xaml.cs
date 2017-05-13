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
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
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
                    reader["ClassName"].ToString()
                    );
                all_projects_listbox.Items.Add(prj);
            }

            cn.Close();
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
