using System;
using System.Collections.Generic;
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
    public partial class FAQ : Page
    {
        private bool user;
        private String userText, staffText, attentionText;
        public FAQ(bool loginWindow = true)
        {
            InitializeComponent();
            // Hide go back button if the user comes from HomeWindow
            if (!loginWindow)
                go_back.Visibility = Visibility.Hidden;

            // Create FAQ text
            attentionText = "ATENTION : If you are lost or have any questions about some action, drag the mouse by the elements and a help message will show up.\n" +
                "For any other questions please contact general@dml.ua.pt\n\n";

            userText = "\t\t\t\t\tHow to request an Electronic Resource?\n" +
                "1) Select the project you want the requisition to be associated with\n" +
                "2) Select the number of units of the desired resource you want to request\n" +
                "3) Click on Request\n" +
                "4) Confirm your action\n" +
                "5) That's it!\n\n" +
                "\t\t\t\t\tHow to request a network resource?\n" +
                "** Virtual Machines **\n" +
                "1) Select the project you want the VM to be associated with\n" +
                "2) Select the OS from the list of available OS's for your machine\n" +
                "3) Choose and write the password to access the VM by ssh\n" +
                "4) Click on Launch VM\n" +
                "5) Confirm your action\n" +
                "6) A message will appear with the ssh command to access your VM\n" +
                "7) That's it!\n\n" +
                "** WiFi LANs **\n" +
                "1) Select the project you the WLAN to be associated with\n" +
                "2) Activate the WLAN\n" +
                "3) Choose and write the password to join your private WLAN\n" +
                "4) Click on Save (or continue requesting if you want some sockets too)\n" +
                "5) Confirm your action\n" +
                "6) That's it!\n\n" +
                "** Sockets **\n" +
                "1) Select the project you want the sockets to be associated with\n" +
                "2) Select the sockets you want to request\n" +
                "3) Click on Save (or continue requesting if you want a WLAN too)\n" +
                "5) Confirm your action\n" +
                "6) That's it!\n\n" + attentionText;

            staffText = "\t\t\t\t\tHow to add units to an equipment?\n" +
                "1) Find the equipment you want to add units\n" +
                "2) Write the name of those units' supplier\n" +
                "3) Pick the number of units you want to add\n" +
                "4) Confirm your action\n" +
                "5) That's it!\n" +
                "\t\t\t\t\tHow to create a new kit?\n" +
                "1) Write the kit name\n" +
                "2) Choose the units that'll belong to the kit\n" +
                "3) Confirm your action\n" +
                "4) That's it!\n" +
                "PS: You can select a kit template by picking an already existing kit and changing their content and/or it's name\n\n" + attentionText;
                
            // Set user text as first to show
            user = true;
            faq_text.Text = userText;
        }

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            // Go back to the login page
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.goToLogin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (user)
            {
                // Switch to staff text
                user = false;
                faq_text.Text = staffText;
                change_button.Content = "User FAQ";
            } else
            {
                // Switch to user text
                user = true;
                faq_text.Text = userText;
                change_button.Content = "Staff FAQ";
            }
        }
    }
}
