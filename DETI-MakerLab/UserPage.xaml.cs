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

        public UserPage(DMLUser User)
        {
            InitializeComponent();
            this.User = User;
            RequisitionsData = new ObservableCollection<Requisition>();
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

            loadRequisitions();
            user_last_requisitions_list.ItemsSource = RequisitionsData;
        }

        private void loadRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            CultureInfo provider = CultureInfo.InvariantCulture;

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", _user.NumMec);
            cmd.CommandText = "dbo.USER_REQS";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                try
                {
                    Requisition r = new Requisition(
                    int.Parse(row["RequisitionID"].ToString()),
                    new Project(
                        int.Parse(row["ProjectID"].ToString()),
                        row["PrjName"].ToString(),
                        row["PrjDescription"].ToString()),
                    null,
                    DateTime.ParseExact(row["ReqDate"].ToString(), "yyMMddHHmm", provider)
                    );
                    r.addResource(new ElectronicUnit(
                        int.Parse(row["ResourceID"].ToString()),
                        new ElectronicResources(
                            row["ProductName"].ToString(),
                            row["Manufactor"].ToString(),
                            row["Model"].ToString(),
                            row["Description"].ToString(),
                            null,
                            row["PathToImage"].ToString()),
                        row["Supplier"].ToString()
                        ));
                    RequisitionsData.Add(r);
                }
                catch (Exception e)
                {
                    foreach (Requisition r in RequisitionsData)
                    {
                        if (r.RequisitionID == int.Parse(row["RequisitionID"].ToString()))
                        {
                            r.addResource(new ElectronicUnit(
                                int.Parse(row["ResourceID"].ToString()),
                                new ElectronicResources(
                                    row["ProductName"].ToString(),
                                    row["Manufactor"].ToString(),
                                    row["Model"].ToString(),
                                    row["Description"].ToString(),
                                    null,
                                    row["PathToImage"].ToString()),
                                row["Supplier"].ToString()
                            ));
                        }
                    }
                }
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
    }
}
