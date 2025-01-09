using PassengerLib;
using System.Windows;
using System.Windows.Input;

namespace Passenger
{
    public partial class UpdateAccount : Window
    {
        public UpdateAccount()
        {
            InitializeComponent(); 
            AccountNameTXT.Text = PassengerLib.Globals.accountName;
            ServiceNameTXT.Text = PassengerLib.Globals.serviceName;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PassengerLib.Globals.closeAppConfirmation = true;
            this.Close();
        }
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void ShowHideNewPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Utils.TextPassBoxChanges.ShowPassword(newPassAccBox, NewPasswordShow);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                Utils.TextPassBoxChanges.HidePassword(newPassAccBox, NewPasswordShow);
            }
        }
        private void GenerateNewPassAcc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            newPassAccBox.Password = PasswordGenerator.GeneratePassword(20);
        }
        private void updateAccPassBTN_Click(object sender, RoutedEventArgs e)
        {
            UpdatePassNotification updatePassNotification = new UpdatePassNotification();
            updatePassNotification.ShowDialog();
            if (PassengerLib.Globals.updatePwdConfirmation)
            {
                PassengerLib.Globals.newAccountPassword = newPassAccBox.Password;
                this.Close();
            }
        }
        private void newPassAccBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            updateAccPassBTN.IsEnabled = (newPassAccBox.Password.Length > 0) ? true : false;
        }
        private void ShowNewPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Utils.TextPassBoxChanges.HidePassword(newPassAccBox, NewPasswordShow);
        }
    }
}
