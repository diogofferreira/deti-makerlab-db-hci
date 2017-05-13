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
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private DMLUser _user;
        private SqlConnection cn;

        public DMLUser User
        {
            get { return _user; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid User");
                _user = value;
            }
        }

        public UserPage(DMLUser User)
        {
            InitializeComponent();
            this.User = User;
            user_name.Text = _user.FirstName + ' ' + _user.LastName;
            user_email.Text = _user.Email;
            user_nmec.Text = _user.NumMec.ToString();
            user_image.Source = new BitmapImage(new Uri(_user.PathToImage, UriKind.Relative));
            if (typeof(Professor).IsInstanceOfType(this.User))
            {
                course_area.Content = "Scientific Area";
                user_course_area.Text = ((Professor)_user).ScientificArea;
            }
            else
            {
                course_area.Content = "Course";
                user_course_area.Text = ((Student)_user).Course;
            }
            //LastRequisitions(int.Parse(this.user_nmec.ToString()));
        }

        private void LastRequisitions(int userID)
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                throw new Exception("Could not connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_USER_REQS_INFO(@userid)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userid", userID);
            SqlDataReader reader = cmd.ExecuteReader();
            CultureInfo provider = CultureInfo.InvariantCulture;
            user_last_requisitions_list.Items.Clear();

            while (reader.Read())
            {
                user_last_requisitions_list.Items.Add(new RequisitionInfo(
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
