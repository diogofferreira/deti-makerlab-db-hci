﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    public partial class ProjectPage : Page
    {
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Requisition> RequisitionsData;
        private Project _project;

        public ProjectPage(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RequisitionsData = new ObservableCollection<Requisition>();
            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;

            // Hardcoded Data
            MembersListData.Add(new Student(78452, "Ana", "Gomes", "ana@ua.pt", "hash", "/images/default-profile.png", "EET"));
            MembersListData.Add(new Student(78452, "Diogo", "Ferreira", "pdiogoferreira@ua.pt", "hash", "/images/default-profile.png", "ECT"));
            MembersListData.Add(new Student(78452, "Pedro", "Martins", "pbmartins@ua.pt", "hash", "/images/default-profile.png", "ECT"));
            MembersListData.Add(new Student(78452, "Rui", "Lemos", "ruilemos@ua.pt", "hash", "/images/default-profile.png", "EET"));
            project_members.ItemsSource = MembersListData;
            project_members.MouseDoubleClick += new MouseButtonEventHandler(project_members_listbox_MouseDoubleClick);
        }

        private void loadRequisitions()
        {
            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            CultureInfo provider = CultureInfo.InvariantCulture;

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand("PROJECT_REQS (@ProjectID)", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                try
                {
                    Requisition r = new Requisition(
                    int.Parse(row["RequisitionID"].ToString()),
                    _project,
                    null,
                    DateTime.ParseExact(row["ReqDate"].ToString(), "yyMMddHHmm", provider)
                    );
                    r.addResource(new ElectronicUnit(
                        int.Parse(row["ResourceID"].ToString()),
                        new ElectronicResources(
                            row["ProductName"].ToString(),
                            row["Manufactor"].ToString(),
                            row["Model"].ToString(),
                            row["Description"].ToString(),
                            null,
                            row["PathToImage"].ToString()),
                        row["Supplier"].ToString()
                        ));
                    RequisitionsData.Add(r);
                }
                catch (Exception e)
                {
                    foreach (Requisition r in RequisitionsData)
                    {
                        if (r.RequisitionID == int.Parse(row["RequisitionID"].ToString()))
                        {
                            r.addResource(new ElectronicUnit(
                                int.Parse(row["ResourceID"].ToString()),
                                new ElectronicResources(
                                    row["ProductName"].ToString(),
                                    row["Manufactor"].ToString(),
                                    row["Model"].ToString(),
                                    row["Description"].ToString(),
                                    null,
                                    row["PathToImage"].ToString()),
                                row["Supplier"].ToString()
                            ));
                        }
                    }
                }
            }
        }

        private SqlConnection getSGBDConnection()
        {
            return new SqlConnection("data source= DESKTOP-H41EV9L\\SQLEXPRESS;integrated security=true;initial catalog=Northwind");
        }

        private bool verifySGBDConnection()
        {
            if (cn == null)
                cn = getSGBDConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        private void project_members_listbox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (project_members.SelectedItem != null)
            {
                DMLUser user = project_members.SelectedItem as DMLUser;
                HomeWindow window = (HomeWindow)Window.GetWindow(this);
                window.goToUserPage(user);
            }
        }

        private void manage_project_button_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow window = (HomeWindow)Window.GetWindow(this);
            window.goToChangeProjectPage(_project);
        }
    }
}
