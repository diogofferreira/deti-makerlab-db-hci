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
    public partial class ClassPage : Page, DMLPages
    {
        private Class _class;
        private SqlConnection cn;
        private ObservableCollection<Requisition> RequisitionsData;

        private Requisition getRequisition(int reqID)
        {
            foreach (Requisition r in RequisitionsData)
                if (r.RequisitionID == reqID)
                    return r;
            return null;
        }

        public ClassPage(Class nClass)
        {
            InitializeComponent();
            this._class = nClass;
            RequisitionsData = new ObservableCollection<Requisition>();
            // Set user's infos
            class_name.Text = _class.ClassName;
            class_description.Text = _class.ClassDescription;
            loadClassManager();

            try
            {
                // Load user's requisitions
                loadRequisitions();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            class_last_requisitions_list.ItemsSource = RequisitionsData;
        }

        private void loadRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@classID", _class.ClassID);
            cmd.CommandText = "SELECT * FROM DML.CLASS_REQS (@classID)";
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                RequisitionsData.Add(new Requisition(
                        int.Parse(reader["RequisitionID"].ToString()),
                        new Project(
                            int.Parse(reader["ProjectID"].ToString()),
                            reader["PrjName"].ToString(),
                            reader["PrjDescription"].ToString(),
                            _class),
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

        private void loadClassManager()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@classID", _class.ClassID);
            cmd.CommandText = "SELECT * FROM DML.LAST_MANAGER (@classID)";
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                class_manager.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
            }
        }

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            // Go back to last page based on current window
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
            if (class_last_requisitions_list.SelectedItem != null)
            {
                Requisition requisition = class_last_requisitions_list.SelectedItem as Requisition;
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToRequisitionPage(requisition);
            }
        }

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
