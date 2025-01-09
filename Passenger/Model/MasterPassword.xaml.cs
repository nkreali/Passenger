using System.Security;
using System.Windows;
using System.Windows.Input;
using PassengerLib;
using Passenger.Utils;

namespace Passenger
{
    public partial class MasterPassword : Window
    {
        public SecureString? masterPassword;
        public MasterPassword()
        {
            InitializeComponent();
            vaultNameLBL.Text = Globals.userName;

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!PasswordValidator.ValidatePassword(masterPasswordPWD.Password))
            {
                Notification.ShowNotificationInfo("orange", "Password must be at least 12 characters, and must include at least one upper case letter, one lower case letter, one numeric digit, one special character and no space!");
                masterPasswordPWD.Clear();
                return;
            }
            
            Globals.masterPasswordCheck = true;
            MasterPasswordTimerStart.MasterPasswordCheck_TimerStart(MainWindow.s_masterPassCheckTimer!);
            masterPassword = masterPasswordPWD.SecurePassword;
            this.Close();
        }
        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void ShowHideMasterPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                TextPassBoxChanges.ShowPassword(masterPasswordPWD, ShowMasterPassword);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                TextPassBoxChanges.HidePassword(masterPasswordPWD, ShowMasterPassword);
            }
        }
        private void ShowPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            TextPassBoxChanges.HidePassword(masterPasswordPWD, ShowMasterPassword);
        }
    }
}
