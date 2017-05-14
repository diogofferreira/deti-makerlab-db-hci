using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RequisitionPage.xaml
    /// </summary>
    public partial class RequisitionPage : Page
    {
        private RequisitionInfo _requisition;

        public RequisitionPage(RequisitionInfo requisition)
        {
            InitializeComponent();
            this._requisition = requisition;
            project_name.Text = _requisition.ProjectName;
            content_list.ItemsSource = _requisition.Resources;
        }

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goBack();
            } catch (Exception exc)
            {
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goBack();
            }
        }
    }
}
