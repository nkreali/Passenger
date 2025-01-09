using System.Windows;

namespace Passenger
{
    public partial class UpdatePassNotification : Window
    {
        public UpdatePassNotification()
        {
            InitializeComponent();
            string account = PassengerLib.Globals.accountName!; 
            notificationLBL.Text = $"Do you want tot update password for {account} account?";
        }
        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.updatePwdConfirmation = true;
            this.Close();
        }
        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.updatePwdConfirmation = false;
            this.Close();
        }

    }
}
