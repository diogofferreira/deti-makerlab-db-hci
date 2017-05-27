using Ookii.Dialogs.Wpf;
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
        private String fileName;

        public Register()
        {
            InitializeComponent();
        }

        private void registerUser(String imagePath)
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Error connecting to database");

            DMLUser user;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userID", int.Parse(nmec.Text));
            cmd.Parameters.AddWithValue("@FirstName", first_name.Text);
            cmd.Parameters.AddWithValue("@LastName", last_name.Text);
            cmd.Parameters.AddWithValue("@Email", email.Text);
            cmd.Parameters.AddWithValue("@PasswordHash", password.Password);
            cmd.Parameters.AddWithValue("@PathToImage", imagePath);
            
            if (user_type.Text.Equals("Student"))
            {
                user = new Student(
                    int.Parse(nmec.Text),
                    first_name.Text, 
                    last_name.Text, 
                    email.Text, 
                    imagePath, 
                    area_or_course_response.Text
                    );
                cmd.Parameters.AddWithValue("@Course", area_or_course_response.Text);
                cmd.CommandText = "dbo.REGISTER_STUDENT";
            }
            else
            {
                user = new Professor(
                    int.Parse(nmec.Text),
                    first_name.Text,
                    last_name.Text,
                    email.Text,
                    imagePath,
                    area_or_course_response.Text
                    );
                cmd.Parameters.AddWithValue("@ScientificArea", area_or_course_response.Text);
                cmd.CommandText = "dbo.REGISTER_PROFESSOR";
            }

            try
            {
                cmd.ExecuteNonQuery();
                user.NumMec = Convert.ToInt32(cmd.Parameters["@userID"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }

        private void checkMandatoryFields()
        {
            if (String.IsNullOrEmpty(email.Text) || String.IsNullOrEmpty(password.Password)
                || String.IsNullOrEmpty(first_name.Text) || String.IsNullOrEmpty(last_name.Text)
                || String.IsNullOrEmpty(nmec.Text) || user_type.SelectedIndex < 0
                || String.IsNullOrEmpty(area_or_course_response.Text) || String.IsNullOrEmpty(user_image.Text))
                throw new Exception("Please fill the mandatory fields!");
        }

        private void upload_image_button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All Image Files | *.*";
            if (dlg.ShowDialog() == true)
            {
                fileName = dlg.FileName;
                user_image.Text = fileName.ToString();
            }
        }

        private void register_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkMandatoryFields();
                // Copy image to project file and produce its path
                String RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                String imagePath = string.Format("{0}images\\", System.IO.Path.GetFullPath(System.IO.Path.Combine(RunningPath, @"..\..\"))) + nmec.Text + System.IO.Path.GetExtension(fileName);
                System.IO.File.Copy(fileName, imagePath, true);
                registerUser(imagePath);
                MessageBox.Show("User successfuly registred!");
                MainWindow window = (MainWindow)Window.GetWindow(this);
                window.goToLogin();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void go_back_button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToLogin();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!user_type.Text.Equals("Student"))
                area_or_course.Content = "Course";
            else
                area_or_course.Content = "Scientific Area";
        }
    }
}
