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
    /// Interaction logic for ResourcesList.xaml
    /// </summary>
    public partial class ResourcesList : Page
    {
        private ObservableCollection<ElectronicResources> EquipmentsListData;
        private ObservableCollection<Kit> KitsListData;
        private SqlConnection cn;

        public ResourcesList()
        {
            InitializeComponent();
            EquipmentsListData = new ObservableCollection<ElectronicResources>();
            KitsListData = new ObservableCollection<Kit>();
            /*
            LoadResources();
            LoadKits();
            */

            // Hardcoded Data
            EquipmentsListData.Add(new ElectronicResources("Raspberry Pi 3",
            "Pi", "Model B", "Raspberry Description", null, "images/rasp.png"));
            EquipmentsListData.Add(new ElectronicResources("Arduino Uno",
            "Adafruit", "Uno", "Arduino Description", null, "images/ard.png"));
            electronics_list.ItemsSource = EquipmentsListData;
            KitsListData.Add(new Kit(1, "Kit Raspberry Pi"));
            KitsListData.Add(new Kit(2, "Kit Arduino Uno"));
            kits_list.ItemsSource = KitsListData;
        }

        private void LoadResources()
        {

            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM ElectronicResource", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ElectronicResources Resource = new ElectronicResources(
                    reader["ProductName"].ToString(),
                    reader["Manufacturer"].ToString(),
                    reader["Model"].ToString(),
                    reader["ResDescription"].ToString(),
                    null,
                    reader["PathToImage"].ToString()
                    );

                EquipmentsListData.Add(Resource);
            }

            cn.Close();
        }

        private void LoadKits()
        {

            cn = getSGBDConnection();
            if (!verifySGBDConnection())
                return;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Kit", cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Kit Resource = new Kit(
                    int.Parse(reader["ResourceID"].ToString()),
                    reader["KitDescription"].ToString()
                    );

                KitsListData.Add(Resource);
            }

            cn.Close();
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

        private void equipment_info_Click(object sender, RoutedEventArgs e)
        {
            ElectronicResources equipment = (ElectronicResources)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goToEquipmentPage(equipment);
        }

        private void kit_info_Click(object sender, RoutedEventArgs e)
        {
            Kit kit = (Kit)(sender as Button).DataContext;
            StaffWindow window = (StaffWindow)Window.GetWindow(this);
            window.goToKitPage(kit);
        }
    }
}
