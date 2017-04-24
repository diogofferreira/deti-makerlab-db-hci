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
            frame.Source = new Uri("Login.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
