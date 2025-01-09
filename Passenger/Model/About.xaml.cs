using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            assemblyNameLBL.Content = $"Name: Passenger";
            versionNameLBL.Content = $"Version: 1.0";
            copyRightLBL.Content = $"© 2025";
            descriptionTXT.Text = $"Simple offline password manager for local storage of sensitive data. " +
                                  $"It uses SQLite DBMS to store user data, AES-256 for encryption and " +
                                  $"Argon2id for storing user password hash.\r\nMade by nkreali.";
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void minimizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void closeBTNMSG_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
