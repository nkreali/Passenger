using System.Windows;
using System.Windows.Input;

namespace Passenger
{
    public partial class MasterPasswordChange : Window
    {
        public MasterPasswordChange()
        {
            InitializeComponent();
            vaultNameTB.Text = PassengerLib.Globals.userName;
            PassengerLib.Globals.closeAppConfirmation = false;
        }

        private void addVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }

        private void ShowVaultPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Utils.TextPassBoxChanges.ShowPassword(OldMasterPassword, OldMasterPasswordTXT);
                Utils.TextPassBoxChanges.ShowPassword(NewMasterPassword, NewMasterPassTXT);
                Utils.TextPassBoxChanges.ShowPassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                Utils.TextPassBoxChanges.HidePassword(OldMasterPassword, OldMasterPasswordTXT);
                Utils.TextPassBoxChanges.HidePassword(NewMasterPassword, NewMasterPassTXT);
                Utils.TextPassBoxChanges.HidePassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
            }
        }

        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.masterPassword = OldMasterPassword.SecurePassword;
            PassengerLib.Globals.newMasterPassword = NewMasterPassword.SecurePassword;
            this.Close();
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        
        private void confirmVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }

        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PassengerLib.Globals.closeAppConfirmation = true;
            this.Close();
        }

        private void ShowNewVaultPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Utils.TextPassBoxChanges.HidePassword(OldMasterPassword, OldMasterPasswordTXT);
            Utils.TextPassBoxChanges.HidePassword(NewMasterPassword, NewMasterPassTXT);
            Utils.TextPassBoxChanges.HidePassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
        }

        private void OldMasterPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }
    }
}
