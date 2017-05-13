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
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectChanges : Page
    {
        private ObservableCollection<DMLUser> MembersListData;
        private Project _project;

        public ProjectChanges(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();

            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            // Hardcoded Data
            MembersListData.Add(new Student(78452, "Rui", "Lemos", "ruilemos@ua.pt", "hash", "none", "ECT"));
            project_members.ItemsSource = MembersListData;
        }

        private void save_project_changes_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The project has been changed!");
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goToProjectPage(_project);
        }
    }
}
