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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        private SqlConnection cn;

        public Register()
        {
            InitializeComponent();
        }

        private void registerUser()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            DMLUser user;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@userID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@FirstName", first_name.Text);
            cmd.Parameters.AddWithValue("@LastName", last_name.Text);
            cmd.Parameters.AddWithValue("@Email", email.Text);
            cmd.Parameters.AddWithValue("@PasswordHash", DMLUser.hashPassword(password.Password));
            cmd.Parameters.AddWithValue("@PathToImage", null);

            if (RadioButton is Student)
            {
                user = new Student(nmec.Text,
                    first_name.Text, 
                    last_name.Text, 
                    email.Text, 
                    DMLUser.hashPassword(password.Password), 
                    null, 
                    course.Text
                    );
                cmd.CommandText = "REGISTER_STUDENT (@FirstName, @LastName, @Email, @PasswordHash, @PathToImage, @Couse, @reqID)";
                cmd.Parameters.AddWithValue("@Course", course.Text);
            } else
            {
                user = new Professor(nmec.Text,
                    first_name.Text,
                    last_name.Text,
                    email.Text,
                    DMLUser.hashPassword(password.Password),
                    null,
                    course.Text
                    );
                cmd.CommandText = "REGISTER_PROFESSOR (@FirstName, @LastName, @Email, @PasswordHash, @PathToImage, @ScientificArea, @reqID)";
                cmd.Parameters.AddWithValue("@ScientificArea", course.Text);
            }

            try
            {
                cmd.ExecuteNonQuery();
                user.NumMec = Convert.ToInt32(cmd.Parameters["@userID"].Value);
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

        private void register_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                registerUser();
                MessageBox.Show("User registed with success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void go_back_button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToLogin();
        }
    }
}
