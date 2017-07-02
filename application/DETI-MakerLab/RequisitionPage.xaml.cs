using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RequisitionPage.xaml
    /// </summary>
    public partial class RequisitionPage : Page, DMLPages
    {
        private Requisition _requisition;
        private SqlConnection cn;

        public RequisitionPage(Requisition requisition)
        {
            InitializeComponent();
            this._requisition = requisition;
            try
            {
                // Get requisition's resources
                LoadRequisitionResources();
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Set requisition infos
            requisition_id.Text = _requisition.RequisitionID.ToString();
            content_list.ItemsSource = _requisition.Resources;
            requisition_user.Text = _requisition.User.FullName;
            requisition_project.Text = _requisition.ReqProject.ProjectName;
            requisition_date.Text = _requisition.ReqDate.ToShortDateString();
        }

        private void LoadRequisitionResources()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Could not connect to database");

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = cn;
            cmd.CommandText = "DML.REQUISITION_UNITS";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@reqID", _requisition.RequisitionID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                _requisition.Resources.Add(new ElectronicUnit(
                    int.Parse(row["ResourceID"].ToString()),
                    new ElectronicResources(
                        row["ProductName"].ToString(),
                        row["Manufacturer"].ToString(),
                        row["Model"].ToString(),
                        row["ResDescription"].ToString(),
                        null,
                        row["PathToImage"].ToString()
                        ),
                    row["Supplier"].ToString()
                    ));
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                _requisition.Resources.Add(new Kit(
                    int.Parse(row["ResourceID"].ToString()),
                    row["KitDescription"].ToString()
                    ));
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

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
