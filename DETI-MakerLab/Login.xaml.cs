using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;

namespace DETI_MakerLab
{
    public partial class Login : Page
    {
        private SqlConnection cn;

        public Login()
        {
            InitializeComponent();
            cn = getSGBDConnection();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //bool logged = checkLogin();

            bool logged = true;
            if (logged) {
                StaffWindow home = new StaffWindow();
                home.Show();
                Window.GetWindow(this).Hide();
            }
            else
                MessageBox.Show("User or password wrong !");
        }

        private bool checkLogin()
        {
            bool result = false;

            if (!verifySGBDConnection())
                return false;

            try {
                SqlCommand cmd = new SqlCommand("SELECT * FROM User");
                cn.Open();
                SqlDataReader dados = cmd.ExecuteReader();
                result = dados.HasRows;
            } catch (SqlException e) {
                throw new Exception(e.Message);
            } finally {
                cn.Close();
            } 

            return result;
        }

        private SqlConnection getSGBDConnection()
        {
            //TODO: fix data source
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

        private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow) Window.GetWindow(this);
            window.goToFAQ();
        }
    }
}
