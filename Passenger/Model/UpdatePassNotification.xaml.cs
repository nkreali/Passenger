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
    /// Логика взаимодействия для UpdatePassNotification.xaml
    /// </summary>
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
