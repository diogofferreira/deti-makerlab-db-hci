using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using Ookii.Dialogs.Wpf;
using System.Text.RegularExpressions;

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
                // Check for valid credentials
                bool logged = checkLogin();
                if (logged)
                {
                    // Go to user's window, based on it's class (user or staff)
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
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    throw new Exception("Cannot connect to database");
                cmd = new SqlCommand("SELECT * FROM DML.CHECK_LOGIN (@email, @password)", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@email", email_box.Text);
                cmd.Parameters.AddWithValue("@password", password_box.Password);
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
                }
                else if (!checkStaff())
                    throw new Exception("User and/or password are incorrect!");
                result = true;

            } catch (SqlException e) {
                throw e;
            } finally {
                cn.Close();
            } 

            return result;
        }

        private bool checkProfessor(DMLUser user)
        {
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database.");
            bool result = false;
            SqlCommand cmdType = new SqlCommand("SELECT * FROM DML.Professor WHERE NumMec=@nummec");
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
                throw new Exception("Cannot connect to database.");
            bool result = false;
            SqlCommand cmdType = new SqlCommand("SELECT * FROM DML.Student WHERE NumMec=@nummec");
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
            cn.Close();
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");
            bool result = false;
            SqlCommand cmd = new SqlCommand("SELECT * FROM DML.CHECK_STAFF_LOGIN (@email, @password)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@email", email_box.Text);
            cmd.Parameters.AddWithValue("@password", password_box.Password);
            SqlDataReader userData = cmd.ExecuteReader();
            if (userData.HasRows)
            {
                userData.Read();
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

        private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Go to register page
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToRegister();
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Go to FAQ page
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToFAQ();
        }
    }
}
