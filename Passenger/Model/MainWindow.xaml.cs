using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PassengerLib;
using Passenger.Utils;
using System.ComponentModel;
using System.Windows.Threading;

namespace Passenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer s_masterPassCheckTimer;
        private static readonly string s_accountName = Environment.UserName;
        private string _userName;
        Mutex? MyMutex;
        public MainWindow()
        {
            Startup();
            InitializeComponent();
            Database.InitializeTables();
            UserManagement.ListUsers(usersList);
            userTXB.Text = " " + s_accountName;
        }
        private void Startup()
        {
            MyMutex = new Mutex(true, "Passenger", out bool aIsNewInstance);
            if (!aIsNewInstance)
            {
                Notification.ShowNotificationInfo("orange", "Passenger is already running...");
                App.Current.Shutdown();
            }
        }

        /// <summary>
        /// Drag window on mouse click left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Passenger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        // Смена вкладок
        private void Users_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewSettings.SetListViewColor(UsersListVI, false);
            ListViewSettings.SetListViewColorApp(AccListVI, true);
            ListViewSettings.SetListViewColorApp(SettingsListVI, true);
            tabControl.SelectedIndex = 0;
        }

        private void Acc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewSettings.SetListViewColor(UsersListVI, true);
            ListViewSettings.SetListViewColorApp(AccListVI, false);
            ListViewSettings.SetListViewColorApp(SettingsListVI, true);
            tabControl.SelectedIndex = 1;
        }

        private void Settings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewSettings.SetListViewColor(UsersListVI, true);
            ListViewSettings.SetListViewColorApp(AccListVI, true);
            ListViewSettings.SetListViewColorApp(SettingsListVI, false);
        }

        private void Sort(string sortBy, ListView listView, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listView.Items);
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        /// <summary>
        /// Minimize button(label)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutBTN_Click(object sender, RoutedEventArgs e)
        {
            var aB = new About();
            aB.ShowDialog();
        }

        private void AddIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddUser addVault = new AddUser();
            addVault.ShowDialog();
            if (Globals.createConfirmation)
            {
                UserManagement.ListUsers(usersList);
            }
        }

        private void LogoutLBL_Click(object sender, RoutedEventArgs e)
        {
            UserManagement.Logout(UsersListVI, AccListVI, SettingsListVI, accList, tabControl, s_masterPassCheckTimer);
            //VaultCloseTimersStop();
        }

        private void usersList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenUsersAccounts();
        }

        private void OpenUsersAccounts()
        {
            var converter = new BrushConverter();
            if (usersList.SelectedItem != null)
            {
                //VaultCloseTimersStop();
                //VaultManagement.VaultClose(vaultsListVI, appListVI, settingsListVI, appList, tabControl, s_masterPassCheckTimer);
                string item = usersList.SelectedItem.ToString()!;
                string userName = item.Split(',')[0].Replace("{ Name = ", "");
                
                var masterPassword = MasterPasswordLoad.LoadMasterPassword(userName);
                Globals.masterPassword = masterPassword;
                if (masterPassword != null && masterPassword.Length > 0)
                {
                    if (AccountManagement.DecryptAndPopulateList(accList, userName, masterPassword))
                    {
                        AccListVI.IsEnabled = true;
                        AccListVI.Foreground = (Brush)converter.ConvertFromString("#FFDCDCDC")!;
                        ListViewSettings.SetListViewColor(UsersListVI, true);
                        ListViewSettings.SetListViewColor(SettingsListVI, true);
                        ListViewSettings.SetListViewColorApp(AccListVI, false);
                        tabControl.SelectedIndex = 1;
                        _userName = userName;

                        AccListUserLVL.Text = userName;
                        //StartTimerVaultClose();
                        Sort("Application", accList, ListSortDirection.Ascending);
                        AccountManagement.AddAccsToTempList(accList);
                    }
                }
            }
            else
            {
                AccListVI.Foreground = Brushes.Red;
                AccListVI.IsEnabled = false;
            }
        }

        private void usersList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenUsersAccounts();
            }
        }

        private void ChangeMasterPassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void accList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddAppIcon_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            //RestartTimerVaultClose();
            AddAccounts addAccounts = new AddAccounts();
            addAccounts.ShowDialog();
            if (Globals.closeAppConfirmation == false)
            {
                if (!Globals.masterPasswordCheck)
                {
                    var masterPassword = MasterPasswordLoad.LoadMasterPassword(_userName);
                    AccountManagement.AddAccount(accList, _userName, Globals.applicationName!, 
                                                 Globals.accountName!, Globals.accountPassword!, masterPassword);
                    AccountManagement.AddAccsToTempList(accList);
                    ClearVariables.VariablesClear();
                    return;
                }

                AccountManagement.AddAccount(accList, _userName, Globals.applicationName!, 
                                             Globals.accountName!, Globals.accountPassword!, Globals.masterPassword!);
                AccountManagement.AddAccsToTempList(accList);
                ClearVariables.VariablesClear();
            }
        }
        private void DelAppIcon_PreviewMouseDown(object sender, RoutedEventArgs e)
        {

        }
        private void AppListColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {

        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UpdateAccountPass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {

        }

       
    }
}