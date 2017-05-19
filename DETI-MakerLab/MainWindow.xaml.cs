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
            frame.Source = new Uri("FAQ.xaml", UriKind.RelativeOrAbsolute);
        }

        public void goToRegister()
        {
            frame.Source = new Uri("Register.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
