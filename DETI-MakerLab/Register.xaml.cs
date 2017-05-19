using System;
using System.Collections.Generic;
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
        private String fileName;

        public Register()
        {
            InitializeComponent();
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

        private void register_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Copy image to project file and produce its path
                String RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                String imagePath = string.Format("{0}images\\", System.IO.Path.GetFullPath(System.IO.Path.Combine(RunningPath, @"..\..\"))) + System.IO.Path.GetFileName(fileName);
                System.IO.File.Copy(fileName, imagePath, true);
                MessageBox.Show("Equipment has been added!");
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                // TODO : register user
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
