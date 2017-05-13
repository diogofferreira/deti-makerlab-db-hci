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
    /// Interaction logic for MyProjects.xaml
    /// </summary>
    public partial class MyProjects : Page
    {
        private SqlConnection cn;

        public MyProjects(int userID)
        {
            InitializeComponent();
            LoadProjects(userID);
        }

        private void LoadProjects(int userID)
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM PROJECT_WORKERS_INFO(@nmec)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@nmec", userID);
            SqlDataReader reader = cmd.ExecuteReader();
            my_projects_listbox.Items.Clear();

            while (reader.Read())
            {
                Project prj = new Project(
                    int.Parse(reader["ProjectID"].ToString()),
                    reader["PrjName"].ToString(),
                    reader["PrjDescription"].ToString(),
                    reader["ClassName"].ToString()
                    );
                my_projects_listbox.Items.Add(prj);
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
