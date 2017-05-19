﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
        private SqlConnection cn;
        private ObservableCollection<DMLUser> MembersListData;
        private ObservableCollection<Role> RolesListData;
        private Project _project;

        public ProjectChanges(Project project)
        {
            InitializeComponent();
            this._project = project;
            MembersListData = new ObservableCollection<DMLUser>();
            RolesListData = new ObservableCollection<Role>();

            project_name.Text = _project.ProjectName;
            project_description.Text = _project.ProjectDescription;
            // Hardcoded Data
            MembersListData.Add(new Student(78452, "Ana", "Gomes", "ana@ua.pt", "hash", "/images/default-profile.png", "EET"));
            MembersListData.Add(new Student(78452, "Diogo", "Ferreira", "pdiogoferreira@ua.pt", "hash", "/images/default-profile.png", "ECT"));
            MembersListData.Add(new Student(78452, "Pedro", "Martins", "pbmartins@ua.pt", "hash", "/images/default-profile.png", "ECT"));
            MembersListData.Add(new Student(78452, "Rui", "Lemos", "ruilemos@ua.pt", "hash", "/images/default-profile.png", "EET"));
            project_members.ItemsSource = MembersListData;
        }

        private void LoadRoles()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            RolesListData.Add(new Role(-1, "Not a Member"));

            SqlCommand cmd = new SqlCommand("SELECT * FROM Roles", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                RolesListData.Add(new Role(
                    int.Parse(reader["RoleID"].ToString()),
                    reader["RoleDescription"].ToString())
                    );
            }

            cn.Close();
        }

        private void LoadMembers()
        {
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                throw new Exception("Cannot connect to database");

            SqlCommand cmd = new SqlCommand("SELECT * FROM DMLUser", cn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<DMLUser> tmp = new List<DMLUser>();

            while (reader.Read())
            {
                DMLUser User = new DMLUser();
                User.NumMec = int.Parse(reader["NumMec"].ToString());
                User.FirstName = reader["FirstName"].ToString();
                User.LastName = reader["LastName"].ToString();
                User.Email = reader["Email"].ToString();
                User.PasswordHash = reader["PasswordHash"].ToString();
                User.PathToImage = reader["PathToImage"].ToString();
                User.RoleID = -1;

                foreach (int[] tuple in _project.Workers)
                {
                    if (tuple[0] == User.NumMec)
                    {
                        User.RoleID = tuple[1];
                        MembersListData.Add(User);
                    }

                }
                if (User.RoleID < 0)
                    tmp.Add(User);
            }

            foreach (DMLUser User in tmp)
                MembersListData.Add(User);

            cn.Close();
        }

        private void saveChanges()
        {
            _project.ProjectDescription = project_description.Text;
            // Save project members
            List<DMLUser> newWorker = new List<DMLUser>();
            List<DMLUser> updateWorker = new List<DMLUser>();
            List<DMLUser> removeWorker = new List<DMLUser>();

            foreach (DMLUser user_item in project_members.Items)
            {
                if (user_item.RoleID != -1)
                {
                    switch (userInProject(user_item))
                    {
                        case 1:
                            updateWorker.Add(user_item);
                            break;

                        case 2:
                            newWorker.Add(user_item);
                            break;

                        default:
                            break;
                    }
                }
                else if (userInProject(user_item) == 1)
                    removeWorker.Add(user_item);
            }

            addWorkers(newWorker);
            updateWorkers(updateWorker);
            removeWorkers(removeWorker);
        }

        private int userInProject(DMLUser user_item)
        {
            foreach (int[] user_tuple in _project.Workers)
            {
                if (user_item.NumMec == user_tuple[0])
                {
                    if (user_item.RoleID == user_tuple[1])
                        return 0;
                    else
                        return 1;
                }
            }
            return 2;
        }

        private void addWorkers(List<DMLUser> newWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in newWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("INSERT INTO WorksOn (UserNMec, ProjectID, UserRole) " +
                    "VALUES (@UserNMec, @ProjectID, @UserRole)", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);
                cmd.Parameters.AddWithValue("@UserRole", user.RoleID);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void updateWorkers(List<DMLUser> updateWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in updateWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("UPDATE WorksOn SET UserRole=@UserRole " +
                    "WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);
                cmd.Parameters.AddWithValue("@UserRole", user.RoleID);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void removeWorkers(List<DMLUser> removeWorker)
        {
            SqlCommand cmd;

            foreach (DMLUser user in removeWorker)
            {
                cn = Helpers.getSGBDConnection();
                if (!Helpers.verifySGBDConnection(cn))
                    throw new Exception("Cannot connect to database");

                cmd = new SqlCommand("DELETE FROM WorksOn " +
                    "WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID", cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserNmec", user.NumMec);
                cmd.Parameters.AddWithValue("@ProjectID", _project.ProjectID);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update contact in database. \n ERROR MESSAGE: \n" + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private void save_project_changes_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //saveChanges();
                MessageBox.Show("The project has been changed!");
                //HomeWindow window = (HomeWindow)Window.GetWindow(this);
                //window.goToProjectPage(_project);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
