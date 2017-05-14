using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private SqlConnection cn;
        private ObservableCollection<Project> ProjectsListData;
        private ObservableCollection<RequisitionInfo> RequisitionsListData;

        public Home()
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            RequisitionsListData = new ObservableCollection<RequisitionInfo>();
            /*
            LoadProjects();
            LoadRequisitions();
            */
            // Hardcoded Data
            ProjectsListData.Add(new Project(1, "DETI MakerLab", "Wiki for DML"));
            ProjectsListData.Add(new Project(2, "BlueConf", "Conference management"));
            RequisitionInfo req1 = new RequisitionInfo(1, "DETI MakerLab", 1, "Raspberry Pi 3 Model B", 2, new DateTime(2017, 5, 13));
            RequisitionInfo req2 = new RequisitionInfo(1, "BlueConf", 1, "Raspberry Pi 3 Model B", 2, new DateTime(2017, 5, 13));
            req1.addResource(1);
            req1.addResource(2);
            req2.addResource(3);
            req2.addResource(4);
            RequisitionsListData.Add(req1);
            RequisitionsListData.Add(req2);

            last_project_list.ItemsSource = ProjectsListData;
            last_requisitions_list.ItemsSource = RequisitionsListData;
        }

        private void LastProjects()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_PROJECTS", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ProjectsListData.Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    int.Parse(reader["ClassID"].ToString())
                    ));
            }
            cn.Close();
        }

        private void LastRequisitions()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_REQUISITIONS_INFO", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            CultureInfo provider = CultureInfo.InvariantCulture;

            while (reader.Read())
            {
                RequisitionsListData.Add(new RequisitionInfo(
                    int.Parse(reader["RequisitionID"].ToString()),
                    reader["PrjName"].ToString(),
                    int.Parse(reader["UserID"].ToString()),
                    reader["ProductDescription"].ToString(),
                    int.Parse(reader["Units"].ToString()),
                    DateTime.ParseExact(reader["ReqDate"].ToString(), "yyMMddHHmm", provider)
                    ));
            }
            cn.Close();
        }

        private SqlConnection getSGBDConnection()
        {
            //TODO: fix data source
            return new SqlConnection("data source= DESKTOP-H41EV9L\\SQLEXPRESS;integrated security=true;initial catalog=DML");
        }

        private bool verifySGBDConnection()
        {
            if (cn == null)
                cn = getSGBDConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        private void project_info_Click(object sender, RoutedEventArgs e)
        {
            Project project = (Project)(sender as Button).DataContext;
            try
            {
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToProjectPage(project);
            } catch (Exception exc)
            {
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goToProjectPage(project);
            }
            
        }

        private void requisition_info_Click(object sender, RoutedEventArgs e)
        {
            RequisitionInfo requisition = (RequisitionInfo)(sender as Button).DataContext;
            try
            {
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToRequisitionPage(requisition);
            } catch (Exception exc)
            {
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goToRequisitionPage(requisition);
            }
        }
    }

}
