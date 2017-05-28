using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
    /// Interaction logic for CreateEquipment.xaml
    /// </summary>
    public partial class CreateEquipment : Page
    {
        private SqlConnection cn;
        private String fileName;
        private Staff _staff;
        private ElectronicResources _equipment;

        public CreateEquipment()
        {
            InitializeComponent();
        }

        public CreateEquipment(Staff staff)
        {
            InitializeComponent();
            this._staff = staff;
        }

        public void SubmitEquipment(String imagePath)
        {
            SqlCommand cmd;
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                return;

            cmd = new SqlCommand("INSERT INTO ElectronicResource (ProductName, Manufacturer, Model, ResDescription, EmployeeNum, PathToImage) " +
                "VALUES (@ProductName, @Manufacturer, @Model, @ResDescription, @EmployeeNum, @PathToImage)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductName", equipment_name.Text);
            cmd.Parameters.AddWithValue("@Manufacturer", equipment_manufacturer.Text);
            cmd.Parameters.AddWithValue("@Model", equipment_model.Text);
            cmd.Parameters.AddWithValue("@ResDescription", equipment_description.Text);
            cmd.Parameters.AddWithValue("@EmployeeNum", _staff.EmployeeNum);
            cmd.Parameters.AddWithValue("@PathToImage", imagePath);

            try
            {
                cmd.ExecuteNonQuery();
                _equipment = new ElectronicResources(
                    equipment_name.Text,
                    equipment_manufacturer.Text,
                    equipment_model.Text,
                    equipment_description.Text,
                    _staff,
                    imagePath
                    );
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
            if (String.IsNullOrEmpty(equipment_name.Text) || String.IsNullOrEmpty(equipment_model.Text)
                || String.IsNullOrEmpty(equipment_manufacturer.Text) || String.IsNullOrEmpty(equipment_image.Text))
                throw new Exception("Please fill the mandatory fields!");
        }

        private void upload_image_button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All Image Files | *.*";
            if (dlg.ShowDialog() == true)
            {
                fileName = dlg.FileName;
                equipment_image.Text = fileName.ToString();
            }
        }

        private void create_equipment_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkMandatoryFields();
                // Copy image to project file and produce its path
                String RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                String name = equipment_name.Text + "_" + equipment_manufacturer.Text + "_" + equipment_model.Text;
                String imagePath = string.Format("{0}images\\", System.IO.Path.GetFullPath(System.IO.Path.Combine(RunningPath, @"..\..\"))) + name + System.IO.Path.GetExtension(fileName);

                MessageBoxResult confirm = MessageBox.Show(
                    "Do you confirm the creation of the equipment?",
                    "Equipment Creation Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
                if (confirm == MessageBoxResult.Yes)
                {
                    SubmitEquipment(imagePath);
                    System.IO.File.Copy(fileName, imagePath, true);
                    MessageBox.Show("Equipment has been successfully added!");
                    StaffWindow window = (StaffWindow)Window.GetWindow(this);
                    window.goToEquipmentPage(_equipment);
                }
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
    }
}
