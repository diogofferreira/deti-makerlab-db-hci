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
        public Home()
        {
            InitializeComponent();
        }
    }

    internal class LastProjects : ObservableCollection<Project>
    {
        private SqlConnection cn;

        public LastProjects()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_PROJECTS", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Add(new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    int.Parse(reader["ClassID"].ToString())
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
    }

    internal class LastRequisitions : ObservableCollection<RequisitionInfo>
    {
        private SqlConnection cn;

        public LastRequisitions()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_REQUISITIONS_INFO", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            CultureInfo provider = CultureInfo.InvariantCulture;

            while (reader.Read())
            {
                Add(new RequisitionInfo(
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
    }
}
