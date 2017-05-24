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
        private ObservableCollection<Requisition> RequisitionsListData;

        private Requisition containsReq(int ReqID)
        {
            foreach (Requisition r in RequisitionsListData)
                if (r.RequisitionID == ReqID)
                    return r;
            return null;
        }

        public Home()
        {
            InitializeComponent();
            ProjectsListData = new ObservableCollection<Project>();
            RequisitionsListData = new ObservableCollection<Requisition>();
            LastProjects();
            LoadUsers();
            LastRequisitions();
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

        private void LastRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Could not connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM LAST_REQUISITIONS";
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

                RequisitionsListData.Add(new Requisition(
                        int.Parse(reader["RequisitionID"].ToString()),
                        new Project(
                            int.Parse(reader["ProjectID"].ToString()),
                            reader["PrjName"].ToString(),
                            reader["PrjDescription"].ToString(),
                            cl),
                        new DMLUser(
                            int.Parse(reader["NumMec"].ToString()),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["Email"].ToString(),
                            reader["PathToImage"].ToString()
                            ),
                        Convert.ToDateTime(reader["ReqDate"])
                    ));

            }
            cn.Close();
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
                MessageBox.Show(exc.Message);

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
