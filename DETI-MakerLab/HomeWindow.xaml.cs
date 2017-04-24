using System;
using System.Windows;

namespace DETI_MakerLab
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();

            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show home page
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Resources_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show collapsed submenu
            this.Resources_Menu.Visibility = this.Resources_Menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Electronics_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show electronics page
            frame.Source = new Uri("Electronics.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Network_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show network page
            frame.Source = new Uri("Network.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Projects_Buttons_Click(object sender, RoutedEventArgs e)
        {
            // Show collapsed submenu
            this.Projects_Menu.Visibility = this.Projects_Menu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Create_Project_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show create project page
            frame.Source = new Uri("CreateProject.xaml", UriKind.RelativeOrAbsolute);
        }

        private void My_Projects_Button_Click(object sender, RoutedEventArgs e)
        {
            // Show my projects page
            frame.Source = new Uri("MyProjects.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
