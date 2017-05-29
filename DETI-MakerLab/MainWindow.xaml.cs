using System;
using System.Windows;
using System.Windows.Navigation;

namespace DETI_MakerLab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Go to login page by default
            goToLogin();
        }

        public void goToLogin()
        {
            // Go to login page
            frame.Source = new Uri("Login.xaml", UriKind.RelativeOrAbsolute);
        }

        public void goToFAQ()
        {
            // Open FAQ section coming from this window
            FAQ page = new FAQ();
            frame.Navigate(page);
        }

        public void goToFAQSection()
        {
            // Open FAQ section coming from HomeWindow
            FAQ page = new FAQ(false);
            frame.Navigate(page);
        }

        public void goToRegister()
        {
            // Go to register page
            frame.Source = new Uri("Register.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
