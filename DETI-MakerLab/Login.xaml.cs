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
            try
            {
                bool logged = checkLogin();
                if (logged)
                {
                    if (user != null)
                    {
                        HomeWindow home = new HomeWindow(user);
                        home.Show();
                    }
                    else if (staff != null)
                    {
                        StaffWindow staffHome = new StaffWindow(staff);
                        staffHome.Show();
                    }
                    Window.GetWindow(this).Hide();
                }
                else
                    MessageBox.Show("User or password wrong !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool checkLogin()
        {
            if (String.IsNullOrEmpty(email_box.Text) || String.IsNullOrEmpty(password_box.Password))
                throw new Exception("You must enter both fields to log in!");
            bool result = false;
            SqlCommand cmd;
            SqlDataReader userData;
            DMLUser tmpUser;

            try {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    return false;
                cmd = new SqlCommand("SELECT * FROM CHECK_LOGIN('" + email_box.Text + "' ,'" + password_box.Password + "')", cn);
                userData = cmd.ExecuteReader();
                if (userData.HasRows)
                {
                    userData.Read();
                    tmpUser = new DMLUser(
                        int.Parse(userData["NumMec"].ToString()),
                        userData["FirstName"].ToString(),
                        userData["LastName"].ToString(),
                        userData["Email"].ToString(),
                        userData["PathToImage"].ToString()
                        );

                    cn.Close();

                    // Check if it is professor, student or staff
                    if (checkProfessor(tmpUser)) { result = true; }
                    if (!result && checkStudent(tmpUser)) { result = true; }
                    if (!result && checkStaff()) { result = true; }
                }
                else
                {
                    throw new Exception("User does not exist!");
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
                    user.PathToImage,
                    typeData["ScientificArea"].ToString()
                    );
                result = true;
            }
            cn.Close();
            return result;
        }

        private bool checkStudent(DMLUser tmpUser)
        {
            if (!Helpers.verifySGBDConnection(cn))
                return false;
            bool result = false;
            SqlCommand cmdType = new SqlCommand("SELECT * FROM Student WHERE NumMec=@nummec");
            cmdType.Parameters.Clear();
            cmdType.Parameters.AddWithValue("@nummec", tmpUser.NumMec);
            cmdType.Connection = cn;
            SqlDataReader typeData = cmdType.ExecuteReader();
            if (typeData.HasRows)
            {
                typeData.Read();
                user = new Student(
                    tmpUser.NumMec,
                    tmpUser.FirstName,
                    tmpUser.LastName,
                    tmpUser.Email,
                    tmpUser.PathToImage,
                    typeData["Course"].ToString()
                    );
                result = true;
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
                    userData["PathToImage"].ToString()
                    );
                result = true;
            }
            cn.Close();
            return result;
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
