using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Passenger.Utils;

namespace Passenger
{
    public partial class AddUser : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public AddUser()
        {
            InitializeComponent();
        }

        private void addVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (confirmVPassword.Password == addVPassword.Password && addVPassword.Password.Length >= 12);
        }

        private void ShowVaultPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                TextPassBoxChanges.ShowPassword(addVPassword, vaultMassterPass);
                TextPassBoxChanges.ShowPassword(confirmVPassword, vaultConfirmMassterPass);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                TextPassBoxChanges.HidePassword(addVPassword, vaultMassterPass);
                TextPassBoxChanges.HidePassword(confirmVPassword, vaultConfirmMassterPass);
            }
        }

        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            UserManagement.CreateUser(vaultNameTXT.Text, addVPassword.Password, confirmVPassword.Password);
            if (PassengerLib.Globals.userChecks)
            {
                TextPassBoxChanges.ClearPBoxesInput(addVPassword, confirmVPassword);
                PassengerLib.Globals.userChecks = false;
            }
            else
            {
                this.Close();
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void confirmVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (confirmVPassword.Password == addVPassword.Password && confirmVPassword.Password.Length >= 12);
        }

        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PassengerLib.Globals.closeAppConfirmation = true;
            this.Close();
        }
        private void ShowNewVaultPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            TextPassBoxChanges.HidePassword(addVPassword, vaultMassterPass);
            TextPassBoxChanges.HidePassword(confirmVPassword, vaultConfirmMassterPass);
        }

        private void vaultNameTXT_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (vaultNameTXT.Text.Length >= 24)
            {
                vaultNameTXT.Text = vaultNameTXT.Text.Substring(0, vaultNameTXT.Text.Length - 1);
                vaultNameTXT.CaretIndex = vaultNameTXT.Text.Length;
                vaultLimitLbl.Content = "User name limit is 24 characters!";
                StartHashLabelClean();
            }
        }

        private void StartHashLabelClean()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick!;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            vaultLimitLbl.Content = " ";
        }
    }
}
