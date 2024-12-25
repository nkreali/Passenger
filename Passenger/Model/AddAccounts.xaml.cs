using PassengerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для AddAccounts.xaml
    /// </summary>
    public partial class AddAccounts : Window
    {
        public AddAccounts()
        {
            InitializeComponent();
        }
        private void addAppBTN_Click(object sender, RoutedEventArgs e)
        {
            PassengerLib.Globals.applicationName = appNameTXT.Text;
            PassengerLib.Globals.accountName = accountNameTXT.Text;
            PassengerLib.Globals.accountPassword = accPasswordBox.Password;
            PassengerLib.Globals.closeAppConfirmation = false;
            Utils.TextPassBoxChanges.ClearTextPassBox(appNameTXT, accountNameTXT, accPasswordBox);
            this.Close();
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
        private void ShowHidePassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Utils.TextPassBoxChanges.ShowPassword(accPasswordBox, PasswordShow);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                Utils.TextPassBoxChanges.HidePassword(accPasswordBox, PasswordShow);
            }
        }
        private void GeneratePassAcc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            accPasswordBox.Password = PasswordGenerator.GeneratePassword(20);
        }
        private void appNameTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utils.TextPassBoxChanges.TextPassBoxChanged(appNameTXT, accountNameTXT, accPasswordBox, addAppBTN);
        }
        private void accountNameTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utils.TextPassBoxChanges.TextPassBoxChanged(appNameTXT, accountNameTXT, accPasswordBox, addAppBTN);
        }
        private void accPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Utils.TextPassBoxChanges.TextPassBoxChanged(appNameTXT, accountNameTXT, accPasswordBox, addAppBTN);
        }
        private void ShowPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Utils.TextPassBoxChanges.HidePassword(accPasswordBox, PasswordShow);
        }
    }
}
