using System;
using System.Collections.Generic;
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
    /// Interaction logic for EquipmentPage.xaml
    /// </summary>
    public partial class EquipmentPage : Page
    {
        private SqlConnection cn;
        private String _productDescription;

        internal String ProductDescription
        {
            get { return _productDescription; }
            set
            {
                if (value == null || String.IsNullOrEmpty(value))
                    throw new Exception("Invalid Product Description");
                _productDescription = value;
            }
        }

        public EquipmentPage(String ProductDescription)
        {
            InitializeComponent();
            this.ProductDescription = ProductDescription;
            loadResource();
        }

        private void loadResource()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM ElectronicResource WHERE ProductName + ' ' + Model = @ProductDescription", cn);
            cmd.Parameters.AddWithValue("@ProductDescription", ProductDescription);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                equipment_name.Text = reader["ProductName"].ToString();
                equipment_model.Text = reader["Model"].ToString();
                equipment_manufacturer.Text = reader["Manufacturer"].ToString();
                equipment_description.Text = reader["ResDescription"].ToString();
                equipment_image.Source = new BitmapImage(new Uri(reader["PathToImage"].ToString(), UriKind.Relative));


            }
            cn.Close();
        }

        private void loadRequisitions()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_EQUIP_REQUISITIONS(@ProductDescription)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductDescription", ProductDescription);
            SqlDataReader reader = cmd.ExecuteReader();
            CultureInfo provider = CultureInfo.InvariantCulture;
            equipment_last_requisitions_list.Items.Clear();

            while (reader.Read())
            {
                equipment_last_requisitions_list.Items.Add(new RequisitionInfo(
                    int.Parse(reader["RequisitionID"].ToString()),
                    reader["PrjName"].ToString(),
                    int.Parse(reader["UserID"].ToString()),
                    null,
                    int.Parse(reader["Units"].ToString()),
                    DateTime.ParseExact(reader["ReqDate"].ToString(), "yyMMddHHmm", provider)
                    ));
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
