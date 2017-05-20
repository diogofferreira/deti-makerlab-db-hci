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
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            user = null;
            staff = null;

            bool logged = checkLogin();

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
            SqlCommand cmd;
            SqlDataReader userData;
            DMLUser user;

            try {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    return false;
                cmd = new SqlCommand("SELECT * FROM DMLUser WHERE Email=@email");
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@email", email_box.Text);
                cmd.Connection = cn;
                userData = cmd.ExecuteReader();
                if (userData.HasRows)
                {
                    userData.Read();
                    user = new DMLUser(
                        int.Parse(userData["NumMec"].ToString()),
                        userData["FirstName"].ToString(),
                        userData["LastName"].ToString(),
                        userData["Email"].ToString(),
                        Convert.ToBase64String((byte[])userData["PasswordHash"]),
                        userData["PathToImage"].ToString()
                        );

                    // Check if password matches
                    if (!user.verifyPassword(password_box.Password))
                        return false;
                    cn.Close();

                    // Check if it is professor or student
                    if (checkProfessor(user)) { result = true; }
                    if (!result && checkStudent(user)) { result = true; }
                }
                else
                {
                    cn.Close();
                    if (checkStaff()) { result = true; }
                }   
            } catch (SqlException e) {
                throw new Exception(e.Message);
            } finally {
                cn.Close();
            } 

            return result;
        }

        private bool checkProfessor(DMLUser user)
        {
            if (!Helpers.verifySGBDConnection(cn))
                return false;
            bool result = false;
            SqlCommand cmdType = new SqlCommand("SELECT * FROM Professor WHERE NumMec=@nummec");
            cmdType.Parameters.Clear();
            cmdType.Parameters.AddWithValue("@nummec", user.NumMec);
            cmdType.Connection = cn;
            SqlDataReader typeData = cmdType.ExecuteReader();
            if (typeData.HasRows)
            {
                typeData.Read();
                user = new Professor(
                    user.NumMec,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PasswordHash,
                    user.PathToImage,
                    typeData["ScientificArea"].ToString()
                    );
                result = true;
            }
            cn.Close();
            return result;
        }

        private bool checkStudent(DMLUser user)
        {
            if (!Helpers.verifySGBDConnection(cn))
                return false;
            bool result = false;
            SqlCommand cmdType = new SqlCommand("SELECT * FROM Student WHERE NumMec=@nummec");
            cmdType.Parameters.Clear();
            cmdType.Parameters.AddWithValue("@nummec", user.NumMec);
            cmdType.Connection = cn;
            SqlDataReader typeData = cmdType.ExecuteReader();
            if (typeData.HasRows)
            {
                typeData.Read();
                user = new Student(
                    user.NumMec,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PasswordHash,
                    user.PathToImage,
                    typeData["Course"].ToString()
                    );
            }
            cn.Close();
            return result;
        }

        private bool checkStaff()
        {
            if (!Helpers.verifySGBDConnection(cn))
                return false;
            bool result = false;
            SqlCommand cmd = new SqlCommand("SELECT * FROM Staff WHERE Email=@email");
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@email", email_box.Text);
            cmd.Connection = cn;
            SqlDataReader userData = cmd.ExecuteReader();
            if (userData.HasRows)
            {
                userData.Read();
                // Check if password matches
                staff = new Staff(
                    int.Parse(userData["EmployeeNum"].ToString()),
                    userData["FirstName"].ToString(),
                    userData["LastName"].ToString(),
                    userData["Email"].ToString(),
                    Convert.ToBase64String((byte[])userData["PasswordHash"]),
                    userData["PathToImage"].ToString()
                    );
                result = staff.verifyPassword(password_box.Password);
            }
            cn.Close();
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

        private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToRegister();
        }
    }
}
