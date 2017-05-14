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
        private Project _project;

        public ProjectPageStatic(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;

            // Hardcoded Data
            MembersListData.Add(new Student(78452, "Rui", "Lemos", "ruilemos@ua.pt", "hash", "/images/dml_logo.png", "ECT"));
            project_members.ItemsSource = MembersListData;
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
    }
}
