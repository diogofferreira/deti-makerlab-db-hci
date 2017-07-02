using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media.Imaging;

namespace DETI_MakerLab
{
    public partial class HomeWindow : Window
    {
        private DMLUser _user;

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

        public HomeWindow(DMLUser user)
        {
            InitializeComponent();

            this.User = user;

            // Set user name label and image
            user_name.Content = _user.FirstName + " " + _user.LastName;
            if (!String.IsNullOrEmpty(_user.PathToImage))
            {
                // Try to load image
                try
                {
                    profile_image.Source = new BitmapImage(new Uri(_user.PathToImage, UriKind.Absolute));
                }
                catch (Exception exc)
                {
                }
            }

            // Show home page
            Home page = new Home(_user.NumMec);
            frame.Navigate(page);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {

            // Check if last page has unsaved infos
            if (!unsavedInfos())
            {
                // Show home page
                Home page = new Home(_user.NumMec);
                frame.Navigate(page);

                // Hide collapsed submenus
                this.resources_menu.Visibility = this.projects_menu.Visibility = Visibility.Collapsed;
            }
        }

        private void Resources_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show/hide collapsed resources submenu
            this.resources_menu.Visibility = this.resources_menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            // Hide collapsed projects submenu
            this.projects_menu.Visibility = Visibility.Collapsed;
        }

        private void Electronics_Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if last page has unsaved infos
            if (!unsavedInfos())
            {
                // Show electronics page
                goToElectronicsPage();
            }
        }

        private void Network_Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if last page has unsaved infos
            if (!unsavedInfos())
            {
                // Show network page
                Network page = new Network(_user.NumMec);
                frame.Navigate(page);
            }
        }

        private void Projects_Buttons_Click(object sender, RoutedEventArgs e)
        {
            // Show/hide collapsed submenu
            this.projects_menu.Visibility = this.projects_menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            // Hide collapsed resources submenu
            this.resources_menu.Visibility = Visibility.Collapsed;
        }

        private void Create_Project_Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if last page has unsaved infos
            if (!unsavedInfos())
            {
                // Show create project page
                CreateProject page = new CreateProject(_user.NumMec);
                frame.Navigate(page);
            }
        }

        private void My_Projects_Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if last page has unsaved infos
            if (!unsavedInfos())
            {
                // Show my projects page
                MyProjects page = new MyProjects(_user.NumMec);
                frame.Navigate(page);
            }
        }

        public void goToProjectPage(Project proj, bool created = false)
        {
            ProjectPage page = new ProjectPage(proj, created);
            frame.Navigate(page);
        }

        public void goToClassPage(Class _class)
        {
            ClassPage page = new ClassPage(_class);
            frame.Navigate(page);
        }

        public void goToProjectStaticPage(Project proj)
        {
            ProjectPageStatic page = new ProjectPageStatic(proj);
            frame.Navigate(page);
        }

        public void goToChangeProjectPage(Project proj)
        {
            ProjectChanges page = new ProjectChanges(proj);
            frame.Navigate(page);
        }

        public void goToEquipmentPage(ElectronicResources equipment)
        {
            EquipmentPage page = new EquipmentPage(equipment);
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

        public void goToElectronicsPage()
        {
            Electronics page = new Electronics(_user.NumMec);
            frame.Navigate(page);
        }

        public void goToKitPage(Kit kit)
        {
            KitPage page = new KitPage(kit);
            frame.Navigate(page);
        }

        public void goBack()
        {
            frame.GoBack();
        }

        private void logout_button_Click(object sender, RoutedEventArgs e)
        {
            // Go to login page on main window
            MainWindow login = new MainWindow();
            login.Show();
            login.goToLogin();
            Window.GetWindow(this).Hide();
        }

        private void user_name_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            goToUserPage(User);
        }

        private void profile_image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            goToUserPage(User);
        }

        private bool unsavedInfos()
        {
            // Check if any fields are filled in
            DMLPages page = (DMLPages)frame.Content;
            if (!page.isEmpty())
            {
                var result = MessageBox.Show("Really want to go back? All changes will be lost", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return true;
            }
            return false;
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Open main window with FAQ section
            MainWindow window = new MainWindow();
            window.Show();
            window.goToFAQSection();
        }
    }
}
