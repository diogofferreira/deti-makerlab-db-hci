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
    /// Interaction logic for CreateEquipment.xaml
    /// </summary>
    public partial class CreateEquipment : Page
    {
        private SqlConnection cn;
        private int _staffID;

        public int StaffID
        {
            get { return _staffID; }
            set { _staffID = value; }
        }

        public CreateEquipment(int StaffID)
        {
            InitializeComponent();
            this.StaffID = StaffID;
        }

        public void SubmitEquipment()
        {
            SqlCommand cmd;
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            cmd = new SqlCommand("INSERT INTO ElectronicResource (ProductName, Manufacturer, Model, ResDescription, EmployeeNum, PathToImage) " +
                "VALUES (@ProductName, @Manufacturer, @Model, @ResDescription, @EmployeeNum, @PathToImage)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductName", equipment_name.Text);
            cmd.Parameters.AddWithValue("@Manufacturer", equipment_manufacturer.Text);
            cmd.Parameters.AddWithValue("@Model", equipment_model.Text);
            cmd.Parameters.AddWithValue("@ResDescription", equipment_description.Text);
            cmd.Parameters.AddWithValue("@EmployeeNum", StaffID.ToString());
            // Rever isto:
            if (typeof(String).IsInstanceOfType(equipment_image))
            {
                cmd.Parameters.AddWithValue("@PathToImage", equipment_image.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@PathToImage", "NULL");
            }

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
