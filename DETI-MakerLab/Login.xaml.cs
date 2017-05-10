using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;

namespace DETI_MakerLab
{
    public partial class Login : Page
    {
        private SqlConnection cn;
        private DMLUser user;
        private Staff staff;

        public Login()
        {
            InitializeComponent();
            cn = getSGBDConnection();
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            user = null;
            staff = null;
            bool logged = checkLogin();

            //bool logged = true;
            if (logged) {
                if (user != null)
                {
                    HomeWindow home = new HomeWindow(user);
                    home.Show();
                } else if (staff != null) {
                    StaffWindow staffHome = new StaffWindow(staff);
                    staffHome.Show();
                }
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
                SqlCommand cmd = new SqlCommand("SELECT * FROM DMLUser WHERE Email=@email");
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@email", email_box.Text);
                cmd.Connection = cn;
                SqlDataReader userData = cmd.ExecuteReader();
                if (userData.HasRows)
                {
                    userData.Read();
                    // Check if password matches
                    verifyPassword(password_box.Password, userData["PasswordHash"].ToString());

                    // Check if it's student or professor
                    SqlCommand cmdType = new SqlCommand("SELECT * FROM Professor WHERE NumMec=@nummec");
                    cmdType.Parameters.Clear();
                    cmdType.Parameters.AddWithValue("@nummec", int.Parse(userData["NumMec"].ToString()));
                    cmdType.Connection = cn;
                    SqlDataReader typeData = cmd.ExecuteReader();
                    if (!typeData.HasRows)
                    {
                        cmd.CommandText = "SELECT * FROM Student WHERE NumMec=@nummec";
                        typeData = cmd.ExecuteReader();
                        user = new Student(
                            int.Parse(userData["NumMec"].ToString()),
                            userData["FirstName"].ToString(),
                            userData["LastName"].ToString(),
                            userData["Email"].ToString(),
                            userData["PasswordHash"].ToString(),
                            userData["PathToImage"].ToString(),
                            typeData["Course"].ToString()
                            );
                    }
                    else
                    {
                        user = new Professor(
                            int.Parse(userData["NumMec"].ToString()),
                            userData["FirstName"].ToString(),
                            userData["LastName"].ToString(),
                            userData["Email"].ToString(),
                            userData["PasswordHash"].ToString(),
                            userData["PathToImage"].ToString(),
                            typeData["ScientificArea"].ToString()
                            );
                    }
                    result = true;  
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM Staff WHERE Email=@email";
                    userData = cmd.ExecuteReader();
                    if (userData.HasRows)
                    {
                        userData.Read();
                        // Check if password matches
                        verifyPassword(password_box.Password, userData["PasswordHash"].ToString());

                        staff = new Staff(
                            int.Parse(userData["EmployeeNum"].ToString()),
                            userData["FirstName"].ToString(),
                            userData["LastName"].ToString(),
                            userData["Email"].ToString(),
                            userData["PasswordHash"].ToString(),
                            userData["PathToImage"].ToString()
                            );
                        result = true;
                    }
                }
                cn.Close();
            } catch (SqlException e) {
                throw new Exception(e.Message);
            } finally {
                cn.Close();
            } 

            return result;
        }

        private void verifyPassword(String password, String savedPasswordHash)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException();
        }


        private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow) Window.GetWindow(this);
            window.goToFAQ();
        }
    }
}
