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
    /// Interaction logic for Electronics.xaml
    /// </summary>
    public partial class Electronics : Page
    {
        SqlConnection cn;
        private int _userID;
        private ObservableCollection<Project> ProjectsListData;		
        private ObservableCollection<ElectronicResources> EquipmentsListData;
        private ObservableCollection<Resources> ActiveRequisitionsData;

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
            ProjectsListData = new ObservableCollection<Project>();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            ActiveRequisitionsData = new ObservableCollection<Resources>();

            // Hardcoded Data
            ProjectsListData.Add(new Project(1, "DETI-MakerLab", "DETI-MakerLab Project Description"));
            ProjectsListData.Add(new Project(2, "BlueConf", "BlueConf Project Description"));
            projects_list.ItemsSource = ProjectsListData;
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3",
            "Pi", "Model B", "Raspberry Description", null, "images/rasp.png"));
            EquipmentsListData.Add(new ElectronicResources("Arduino Uno",
            "Adafruit", "Uno", "Arduino Description", null, "images/ard.png"));
            equipment_list.ItemsSource = EquipmentsListData;
            ActiveRequisitionsData.Add(new Resources(4, "Raspberry Pi 3 - ID#4"));
            ActiveRequisitionsData.Add(new Resources(5, "Arduino Uno - ID#5"));
            active_requisitions_list.ItemsSource = ActiveRequisitionsData;
            //LoadProjects();
            //LoadAvailableResources();
            //LoadProjectActiveRequisitons();
        }

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
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
            
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM PROJECT_ACTIVE_REQS(@projectID)", cn);
            cmd.Parameters.AddWithValue("@projectID", SelectedProject.ProjectID);
            SqlDataReader reader = cmd.ExecuteReader();
            active_requisitions_list.Items.Clear();

            while (reader.Read())
            {
                Resources unit = new Resources(
                    int.Parse(reader["ResourceID"].ToString()),
                    reader["ProductDescription"].ToString()
                    );
                active_requisitions_list.Items.Add(unit);
            }

            cn.Close();
        }

        /* Precisamos de ver como fica lista para tirar os valores de lá
        private void SubmitRequisitionResources()
        {
            bool createdRequisition = false;
            int reqID = -1;

            if (SelectedProject == null)
                return;

            foreach (ListBoxItem resource in equipment_list.Items)
            {
                if (int.Parse(resource.units) == 0)
                    continue;

                if (!createdRequisition)
                {
                    reqID = SubmitRequisition();
                    createdRequisition = true;
                }
                if (createdRequisition && reqID == -1)
                    throw new Exception("Error creating requisition");

                cn = getSGBDConnection();
                if (!verifySGBDConnection())
                    throw new Exception("Error connecting to database"); ;

                SqlCommand cmd = new SqlCommand("REQUEST_RESOURCES(@productDescription, @qty, @reqID)", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productDescription", resource.ProductDescription);
                cmd.Parameters.AddWithValue("@qty", int.Parse(resource.units));
                cmd.Parameters.AddWithValue("@reqID", reqID);

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

            if (!createdRequisition)
                throw new Exception("Cannot make a requisition of none equipments!");
        }*/

        private int SubmitRequisition()
        {
            int reqID = -1;
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return reqID;

            SqlCommand cmd = new SqlCommand("INSERT INTO Requisition(ProjectID, UserID, ReqDate) " +
                "OUTPUT Inserted.RequisitionID VALUES (@ProjectID, @UserID, @ReqDate)", cn);
            cmd.Parameters.AddWithValue("@ProjectID", SelectedProject.ProjectID);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@ReqDate", "CURRENT_TIMESTAMP");

            try
            {
                reqID = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
            }
            finally
            {
                cn.Close();
            }
            return reqID;
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

        private void request_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Request done with sucess !");
        }

        private void deliver_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delivery done with sucess !");
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
