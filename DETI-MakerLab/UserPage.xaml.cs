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
        private ObservableCollection<Requisition> RequisitionsData;

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

        private Requisition getRequisition(int reqID)
        {
            foreach (Requisition r in RequisitionsData)
                if (r.RequisitionID == reqID)
                    return r;
            return null;
        }

        public UserPage(DMLUser User)
        {
            InitializeComponent();
            this.User = User;
            RequisitionsData = new ObservableCollection<Requisition>();
            user_name.Text = _user.FirstName + ' ' + _user.LastName;
            user_email.Text = _user.Email;
            user_nmec.Text = _user.NumMec.ToString();
            user_image.Source = new BitmapImage(new Uri(_user.PathToImage, UriKind.Absolute));
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
            try
            {
                loadRequisitions();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            user_last_requisitions_list.ItemsSource = RequisitionsData;
        }

        private void loadRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@userID", User.NumMec);
            cmd.CommandText = "SELECT * FROM USER_REQS (@userID)";
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Class cl = null;
                if (reader["ClassID"] != DBNull.Value)
                    cl = new Class(
                        int.Parse(reader["ClassID"].ToString()),
                        reader["ClassName"].ToString(),
                        reader["ClDescription"].ToString()
                    );

                RequisitionsData.Add(new Requisition(
                        int.Parse(reader["RequisitionID"].ToString()),
                        new Project(
                            int.Parse(reader["ProjectID"].ToString()),
                            reader["PrjName"].ToString(),
                            reader["PrjDescription"].ToString(),
                            cl),
                        new DMLUser(
                            int.Parse(reader["NumMec"].ToString()),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["Email"].ToString(),
                            reader["PathToImage"].ToString()
                            ),
                        Convert.ToDateTime(reader["ReqDate"])
                    ));
            }
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

        private void user_last_requisitions_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (user_last_requisitions_list.SelectedItem != null)
            {
                Requisition requisition = user_last_requisitions_list.SelectedItem as Requisition;
                try
                {
                    HomeWindow window = (HomeWindow)Window.GetWindow(this);
                    window.goToRequisitionPage(requisition);
                } catch (Exception exc)
                {
                    StaffWindow window = (StaffWindow)Window.GetWindow(this);
                    window.goToRequisitionPage(requisition);
                }
                
            }
        }
    }
}
