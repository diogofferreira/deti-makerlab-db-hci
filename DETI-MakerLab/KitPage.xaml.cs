using Ookii.Dialogs.Wpf;
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
    /// Interaction logic for KitPage.xaml
    /// </summary>
    public partial class KitPage : Page, DMLPages
    {
        private Kit _kit;
        private SqlConnection cn;
        private ObservableCollection<Requisition> RequisitionsData;

        public KitPage(Kit kit, bool created = false)
        {
            InitializeComponent();
            if (created)
                go_back.Visibility = Visibility.Hidden;
            RequisitionsData = new ObservableCollection<Requisition>();
            _kit = kit;
            kit_name.Text = _kit.Description;
            kit_id.Text = _kit.ResourceID.ToString();
            content_list.ItemsSource = kit.Units;
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
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            equipment_last_requisitions_list.ItemsSource = RequisitionsData;
        }

        private void loadRequisitions()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM LAST_KIT_REQUISITIONS (@KitDescription)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@KitDescription", _kit.Description);
            SqlDataReader reader = cmd.ExecuteReader();
            CultureInfo provider = CultureInfo.InvariantCulture;

            while (reader.Read())
            {
                try
                {
                    Requisition r = new Requisition(
                    int.Parse(reader["RequisitionID"].ToString()),
                    new Project(
                        int.Parse(reader["ProjectID"].ToString()),
                        reader["PrjName"].ToString(),
                        reader["PrjDescription"].ToString()),
                    null,
                    DateTime.ParseExact(reader["ReqDate"].ToString(), "yyMMddHHmm", provider)
                    );
                    r.addResource(new ElectronicUnit(
                        int.Parse(reader["ResourceID"].ToString()),
                        new ElectronicResources(
                            reader["ProductName"].ToString(),
                            reader["Manufactor"].ToString(),
                            reader["Model"].ToString(),
                            reader["Description"].ToString(),
                            null,
                            reader["PathToImage"].ToString()),
                        reader["Supplier"].ToString()
                        ));
                    RequisitionsData.Add(r);
                }
                catch (Exception e)
                {
                    foreach (Requisition r in RequisitionsData)
                    {
                        if (r.RequisitionID == int.Parse(reader["RequisitionID"].ToString()))
                        {
                            r.addResource(new ElectronicUnit(
                                int.Parse(reader["ResourceID"].ToString()),
                                new ElectronicResources(
                                    reader["ProductName"].ToString(),
                                    reader["Manufactor"].ToString(),
                                    reader["Model"].ToString(),
                                    reader["Description"].ToString(),
                                    null,
                                    reader["PathToImage"].ToString()),
                                reader["Supplier"].ToString()
                            ));
                            break;
                        }
                    }
                }
            }
            cn.Close();
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

        public bool isEmpty()
        {
            // There are no fields to check
            return true;
        }
    }
}
