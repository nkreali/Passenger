using Passenger.Model;
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
    /// Логика взаимодействия для UpdateAccount.xaml
    /// </summary>
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
