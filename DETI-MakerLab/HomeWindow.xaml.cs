using System;
using System.Windows;

namespace DETI_MakerLab
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            frame.Source = new Uri("Home.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Resources_Button_Click(object sender, RoutedEventArgs e)
        {
            frame.Source = new Uri("Network.xaml", UriKind.RelativeOrAbsolute);
        }

        private void Projects_Buttons_Click(object sender, RoutedEventArgs e)
        {
            frame.Source = new Uri("Projects.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
