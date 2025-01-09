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
using System.Windows.Shapes;

namespace Passenger
{
    /// <summary>
    /// Логика взаимодействия для DeleteAccount.xaml
    /// </summary>
    public partial class DeleteAccount : Window
    {
        public DeleteAccount()
        {
            InitializeComponent(); 
            string service = PassengerLib.Globals.serviceName!;
            string account = PassengerLib.Globals.accountName!;
            notificationLBL.Text = $"Do you want to delete {account} account for {service} service?";
        }
        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.deleteConfirmation = true;
            this.Close();
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.serviceName = "";
            PassengerLib.Globals.accountName = "";
            PassengerLib.Globals.deleteConfirmation = false;
            this.Close();
        }
    }
}
