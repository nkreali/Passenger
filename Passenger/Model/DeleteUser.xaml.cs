using System.Windows;

namespace Passenger
{
    public partial class DeleteUser : Window
    {
        public DeleteUser()
        {
            InitializeComponent();
            string userName = PassengerLib.Globals.userName!;
            notificationLBL.Text = $"Do you want tot delete/remove {userName} user?";
        }

        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.deleteConfirmation = true;
            this.Close();
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.deleteConfirmation = false;
            this.Close();
        }
    }
}
