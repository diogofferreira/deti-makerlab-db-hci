using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;

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
            //profile_image.Source = ;

            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        public void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
            
            // Hide collapsed submenus
            this.resources_menu.Visibility = this.projects_menu.Visibility =  Visibility.Collapsed;
        }

        public void Resources_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show/hide collapsed resources submenu
            this.resources_menu.Visibility = this.resources_menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            // Hide collapsed projects submenu
            this.projects_menu.Visibility = Visibility.Collapsed;
        }

        public void Electronics_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show electronics page
            Electronics page = new Electronics(_user.NumMec);
            frame.Navigate(page);
        }

        public void Network_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show network page
            frame.Source = new Uri("Network.xaml", UriKind.RelativeOrAbsolute);
        }

        public void Projects_Buttons_Click(object sender, RoutedEventArgs e)
        {
            // Show/hide collapsed submenu
            this.projects_menu.Visibility = this.projects_menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            // Hide collapsed resources submenu
            this.resources_menu.Visibility = Visibility.Collapsed;
        }

        public void Create_Project_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show create project page
            frame.Source = new Uri("CreateProject.xaml", UriKind.RelativeOrAbsolute);
        }

        public void My_Projects_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show my projects page
            frame.Source = new Uri("MyProjects.xaml", UriKind.RelativeOrAbsolute);
        }

        public void goToProjectPage(Project proj)
        {
            ProjectPage page = new ProjectPage(proj);
            frame.Navigate(page);
        }

        public void goToChangeProjectPage(Project proj)
        {
            ProjectChanges page = new ProjectChanges(proj);
            frame.Navigate(page);
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
