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
            goToLogin();
        }

        public void goToLogin()
        {
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
            frame.Source = new Uri("Register.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
