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
using System.Windows.Shapes;

namespace DETI_MakerLab
{
    /// <summary>
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        private Staff _user;

        internal Staff StaffUser
        {
            get { return _user; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid User");
                _user = value;
            }
        }

        public StaffWindow()
        {
            InitializeComponent();
        }

        public StaffWindow(Staff User)
        {
            InitializeComponent();
            this.StaffUser = User;
            
            // Set user name label and image
            user_name.Content = _user.FirstName + " " + _user.LastName;
            if (!String.IsNullOrEmpty(_user.PathToImage))
                profile_image.Source = new BitmapImage(new Uri(_user.PathToImage, UriKind.Absolute));

            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);

            // Hide collapsed submenus
            this.resources_menu.Visibility = Visibility.Collapsed;
        }

        private void Resources_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show/hide collapsed resources submenu
            this.resources_menu.Visibility = this.resources_menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void resources_list_button_Click(object sender, RoutedEventArgs e)
        {
            // Show resources list page
            frame.Source = new Uri("ResourcesList.xaml", UriKind.RelativeOrAbsolute);
        }

        private void add_equipment_button_Click(object sender, RoutedEventArgs e)
        {
            // Show create equipment page
            CreateEquipment page = new CreateEquipment(_user.EmployeeNum);
            frame.Navigate(page);
        }

        private void add_unit_button_Click(object sender, RoutedEventArgs e)
        {
            // Show add unit page
            AddUnit page = new AddUnit(_user);
            frame.Navigate(page);
        }

        private void add_kit_button_Click(object sender, RoutedEventArgs e)
        {
            // Show add kit page
            CreateKit page = new CreateKit(_user.EmployeeNum);
            frame.Navigate(page);
        }

        private void Projects_Buttons_Click(object sender, RoutedEventArgs e)
        {
            // Show projects page
            frame.Source = new Uri("AllProjects.xaml", UriKind.RelativeOrAbsolute);

            // Hide collapsed submenus
            this.resources_menu.Visibility = Visibility.Collapsed;
        }

        public void goToEquipmentPage(ElectronicResources equipment)
        {
            EquipmentPage page = new EquipmentPage(equipment);
            frame.Navigate(page);
        }

        public void goToKitPage(Kit kit)
        {
            KitPage page = new KitPage(kit);
            frame.Navigate(page);
        }

        public void goToProjectPage(Project proj)
        {
            ProjectPageStatic page = new ProjectPageStatic(proj);
            frame.Navigate(page);
        }

        public void goToUserPage(DMLUser user)
        {
            UserPage page = new UserPage(user);
            frame.Navigate(page);
        }

        public void goToRequisitionPage(Requisition req)
        {
            RequisitionPage page = new RequisitionPage(req);
            frame.Navigate(page);
        }

        public void goBack()
        {
            frame.GoBack();
        }

        private void logout_button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            login.goToLogin();
            Window.GetWindow(this).Hide();
        }
    }
}
