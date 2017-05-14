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
        private ElectronicResources _equipment;

        internal ElectronicResources Equipment
        {
            get { return _equipment; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid Product Description");
                _equipment = value;
            }
        }

        public EquipmentPage(ElectronicResources equipment)
        {
            InitializeComponent();
            this._equipment = equipment;
            equipment_name.Text = _equipment.ProductName;
            equipment_model.Text = _equipment.Model;
            equipment_manufacturer.Text = _equipment.Manufactor;
            equipment_description.Text = _equipment.Description;
            equipment_image.Source = new BitmapImage(new Uri(_equipment.PathToImage, UriKind.Relative));
            //loadRequisitions();
        }

        private void loadRequisitions()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_EQUIP_REQUISITIONS(@ProductDescription)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductDescription", _equipment.Description);
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

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goBack();
            } catch (Exception exc)
            {
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goBack();
            }            
        }
    }
}
