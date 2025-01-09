using System.Windows;

namespace Passenger
{
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
