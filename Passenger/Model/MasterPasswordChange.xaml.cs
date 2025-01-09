using Microsoft.Win32;
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
    /// Логика взаимодействия для MasterPasswordChenge.xaml
    /// </summary>
    public partial class MasterPasswordChange : Window
    {
        public MasterPasswordChange()
        {
            InitializeComponent();
            vaultNameTB.Text = PassengerLib.Globals.userName;
            PassengerLib.Globals.closeAppConfirmation = false;
        }

        /// <summary>
        /// Check password length and enable create vault button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }
        /// <summary>
        /// Show/hide master password from create vault passwordbox using a textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Mouse window drag function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Check password length and enable create button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }

        /// <summary>
        /// Close button label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PassengerLib.Globals.closeAppConfirmation = true;
            this.Close();
        }

        /// <summary>
        /// Hide master password when mouse is moved over from eye icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowNewVaultPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Utils.TextPassBoxChanges.HidePassword(OldMasterPassword, OldMasterPasswordTXT);
            Utils.TextPassBoxChanges.HidePassword(NewMasterPassword, NewMasterPassTXT);
            Utils.TextPassBoxChanges.HidePassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
        }

        /// <summary>
        ///  Check old password length and enable create button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OldMasterPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }
    }
}
