using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DETI_MakerLab
{
    class Helpers
    {
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static SqlConnection getSGBDConnection()
        {
            return new SqlConnection("data source= DESKTOP-H41EV9L\\SQLEXPRESS;integrated security=true;initial catalog=DML");
        }

        public static bool verifySGBDConnection(SqlConnection cn)
        {
            if (cn == null)
                cn = getSGBDConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        public static void ShowCustomDialogBox(Exception ex)
        {
            // Create custom message box dialog
            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.WindowTitle = "Error";
                dialog.MainInstruction = "Database Error";
                dialog.Content = "An error occurred while performing a database operation. See details to analyse the specific error";
                dialog.ExpandedInformation = ex.Message;
                TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
                dialog.Buttons.Add(okButton);
                dialog.Show();
            }
        }
    }
}
