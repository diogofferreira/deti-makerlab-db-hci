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
        private ObservableCollection<Resources> RequisitionsListData;

        public Home()
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            RequisitionsListData = new ObservableCollection<Resources>();
            /*
            LoadProjects();
            LoadRequisitions();
            */
            // Hardcoded Data
            ProjectsListData.Add(new Project(1, "DETI MakerLab", "Wiki for DML"));
            ProjectsListData.Add(new Project(2, "BlueConf", "Conference management"));
            //RequisitionInfo req1 = new RequisitionInfo(1, "DETI MakerLab", 1, "Raspberry Pi 3 Model B", 2, new DateTime(2017, 5, 13));
            //RequisitionInfo req2 = new RequisitionInfo(1, "BlueConf", 1, "Raspberry Pi 3 Model B", 2, new DateTime(2017, 5, 13));
            //req1.addResource(1);
            //req1.addResource(2);
            //req2.addResource(3);
            //req2.addResource(4);
            //RequisitionsListData.Add(req1);
            //RequisitionsListData.Add(req2);

            last_project_list.ItemsSource = ProjectsListData;
            last_requisitions_list.ItemsSource = RequisitionsListData;
        }

        private void LastProjects()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_PROJECTS", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ProjectsListData.Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    )));
            }
            cn.Close();
        }

        private void LastRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Could not connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand("LAST_REQUISITIONS", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                RequisitionsListData.Add(new ElectronicUnit(
                    int.Parse(row["ResourceID"].ToString()),
                    new ElectronicResources(
                        row["ProductName"].ToString(),
                        row["Manufactor"].ToString(),
                        row["Model"].ToString(),
                        row["Description"].ToString(),
                        null,
                        row["PathToImage"].ToString()
                        ),
                    row["Supplier"].ToString()
                    ));
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                RequisitionsListData.Add(new Kit(
                    int.Parse(row["ResourceID"].ToString()),
                    row["KitDescription"].ToString()
                    ));
            }
        }

        // REVER A QUESTÂO DO QUE MOSTRAR NAS REQUISIÇÕES

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
            Requisition requisition = (Requisition)(sender as Button).DataContext;
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
