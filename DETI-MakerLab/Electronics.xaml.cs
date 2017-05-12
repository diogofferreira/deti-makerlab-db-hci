using System;
using System.Collections.Generic;
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
    /// Interaction logic for Electronics.xaml
    /// </summary>
    public partial class Electronics : Page
    {
        SqlConnection cn;
        private int _userID;
        private Project _selectedProject;

        internal int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        internal Project SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }

        public Electronics(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
            this.SelectedProject = null;
            LoadProjects();
        }

        private void LoadProjects()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM PROJECT_WORKERS_INFO WHERE UserNMec=@nmec", cn);
            cmd.Parameters.AddWithValue("@nmec", UserID);
            SqlDataReader reader = cmd.ExecuteReader();
            projects_list.Items.Clear();

            while (reader.Read())
            {
                Project prj = new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    reader["ClassName"].ToString()
                    );
                projects_list.Items.Add(prj);
            }

            cn.Close();
        }

        private void LoadAvailableResources()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM RESOURCES_TO_REQUEST", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            equipment_list.Items.Clear();

            while (reader.Read())
            {
                ResourceInfo res = new ResourceInfo(
                    reader["ProductDescription"].ToString(),
                    int.Parse(reader["AvailableUnits"].ToString())
                    );
                equipment_list.Items.Add(res);
            }

            cn.Close();
        }

        private void LoadProjectActiveRequisitons()
        {
            if (SelectedProject == null)
                return;
            int projectID = SelectedProject.ProjectID;
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM PROJECT_ACTIVE_REQS(@projectID)", cn);
            cmd.Parameters.AddWithValue("@projectID", SelectedProject.ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();
            projects_list.Items.Clear();

            while (reader.Read())
            {
                Resources unit = new Resources(
                    int.Parse(reader["ResourceID"].ToString()),
                    reader["ProductDescription"].ToString()
                    );
                projects_list.Items.Add(unit);
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

    [Serializable()]
    internal class ResourceInfo
    {
        private String _productDescription;
        private int _units;

        internal String ProductDescription
        {
            get { return _productDescription; }
            set { _productDescription = value; }
        }

        internal int Units
        {
            get { return _units; }
            set { _units = value; }
        }

        public ResourceInfo(String ProductDescription, int Units)
        {
            this.ProductDescription = ProductDescription;
            this.Units = Units;
        }

    }
}
