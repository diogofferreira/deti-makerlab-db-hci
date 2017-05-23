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
    /// Interaction logic for ProjectPageStatic.xaml
    /// </summary>
    public partial class ProjectPageStatic : Page
    {
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<DMLUser> RequisitionsData;

        private Project _project;

        public ProjectPageStatic(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RequisitionsData = new ObservableCollection<DMLUser>();
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            project_members.ItemsSource = MembersListData;
            project_last_requisitions_list.ItemsSource = RequisitionsData;
            project_members.MouseDoubleClick += new MouseButtonEventHandler(project_members_listbox_MouseDoubleClick);
        }

        private void project_members_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (project_members.SelectedItem != null)
            {
                DMLUser user = project_members.SelectedItem as DMLUser;
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                window.goToUserPage(user);
            }
        }

        private void go_back_button_Click(object sender, RoutedEventArgs e)
        {
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goBack();
        }
    }
}
