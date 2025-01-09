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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Passenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string s_accountName = Environment.UserName;
        private string? _userName;
        private DispatcherTimer? _dispatcherTimer;
        private DispatcherTimer? _dispatcherTimerLogout;
        private DispatcherTimer? _dispatcherTimerElapsed;
        public static DispatcherTimer? s_masterPassCheckTimer;
        private int _CloseSession = 0;
        Mutex? MyMutex;
        public MainWindow()
        {
            Startup();
            InitializeComponent(); 
            Database.InitializeTables();
            s_masterPassCheckTimer = new DispatcherTimer();
            UserManagement.ListUsers(usersList);
            userTXB.Text = " " + s_accountName; 
            ListViewSettings.SetListViewColor(UsersListVI, false);
            ListViewSettings.SetListViewColorApp(AccListVI, true); 
            UserSessionExpire.LoadExpireTime(Globals.registryPath, Globals.userExpireReg, "10", expirePeriodTxT);
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
            tabControl.SelectedIndex = 2;
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
            UserManagement.Logout(UsersListVI, AccListVI, SettingsListVI, accList, tabControl, s_masterPassCheckTimer!);
            UserLogoutTimersStop();
        }

        private void OpenUsersAccounts()
        {
            var converter = new BrushConverter();
            if (usersList.SelectedItem != null)
            {
                UserLogoutTimersStop();
                UserManagement.Logout(UsersListVI, AccListVI, SettingsListVI, accList, tabControl, s_masterPassCheckTimer!);
                
                User? user = (User)usersList.SelectedItem;
                string userName = user.Name!;
                
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
                        Globals.vaultOpen = true;
                        StartTimerVaultClose();
                        Sort("Application", accList, ListSortDirection.Ascending);
                    }
                }
            }
            else
            {
                AccListVI.Foreground = Brushes.Red;
                AccListVI.IsEnabled = false;
            }
        }
        private void usersList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenUsersAccounts();
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
            if (usersList.SelectedIndex == -1)
            {
                Notification.ShowNotificationInfo("orange", "You must select a user for changing Master Password!");
                return;
            }
            Globals.userName = UserManagement.GetUserNameFromListView(usersList);
            if (Globals.vaultOpen)
            {
                Notification.ShowNotificationInfo("orange", "You cannot change Master Password when the user is active!");
                return;
            }
            var mPasswordChanger = new MasterPasswordChange();
            mPasswordChanger.ShowDialog();
            if (Globals.closeAppConfirmation == false)
            {
                UserManagement.ChangeMasterPassword(usersList);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.vaultOpen)
            {
                Notification.ShowNotificationInfo("orange", "You cannot delete when the user is active!");
                return;
            }

            UserLogoutTimersStop();
            UserManagement.DeleteUserItem(usersList);

        }

        private void DeleteIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Globals.vaultOpen)
            {
                Notification.ShowNotificationInfo("orange", "You cannot delete when the user is active!");
                return;
            }

            UserLogoutTimersStop();
            UserManagement.DeleteUserItem(usersList);
        }

        
        private void AddAppIcon_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            AddAccounts addAccounts = new AddAccounts();
            addAccounts.ShowDialog();
            if (Globals.closeAppConfirmation == false)
            {
                if (!Globals.masterPasswordCheck)
                {
                    var masterPassword = MasterPasswordLoad.LoadMasterPassword(_userName!);
                    AccountManagement.AddAccount(accList, _userName!, Globals.serviceName!, 
                                                 Globals.accountName!, Globals.accountPassword!, masterPassword);
                    ClearVariables.VariablesClear();
                    return;
                }

                AccountManagement.AddAccount(accList, _userName!, Globals.serviceName!, 
                                             Globals.accountName!, Globals.accountPassword!, Globals.masterPassword!);
                ClearVariables.VariablesClear();
            }
        }
        private void DelAccIcon_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            AccountManagement.DeleteSelectedItem(accList, _userName!, usersList);

        }
        private void AccListColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClipBoardUtil.ClearClipboard(Globals.accountPassword!);
            Globals.accountPassword = null;
            _dispatcherTimer!.Stop();
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            if (accList.SelectedIndex == -1)
            {
                Notification.ShowNotificationInfo("orange", "You must select a application account for Copy to Clipboard option!");
                return;
            }

            Clipboard.SetText(AccountManagement.CopyPassToClipBoard(accList));
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += dispatcherTimer_Tick!;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            _dispatcherTimer.Start();
        }
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            AccountManagement.ShowPassword(accList);            
        }
        private void UpdateAccountPass_Click(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            if (accList.SelectedIndex == -1)
            {
                Notification.ShowNotificationInfo("orange", "You must select a application line for updateing account password!");
                return;
            }
            AccountManagement.UpdateSelectedItemPassword(accList, _userName!);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            RestartTimerUserLogout();
            AccountManagement.DeleteSelectedItem(accList, _userName!, usersList);
        }

        private void applyExpirePeriodBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (expirePeriodTxT.Text.Length > 0 && !expirePeriodTxT.Text.StartsWith("0"))
                {
                    Regex regexNumber = new Regex("[^0-9.-]+");
                    if (!regexNumber.IsMatch(expirePeriodTxT.Text) && !expirePeriodTxT.Text.Contains("-"))
                    {
                        int expireTime = Int32.Parse(expirePeriodTxT.Text);
                        RegistryManagement.RegKey_WriteSubkey(Globals.registryPath, Globals.userExpireReg, expirePeriodTxT.Text);
                        Globals.sessionExpireInterval = expireTime;
                        Notification.ShowNotificationInfo("green", $"User session expire time is set to {expirePeriodTxT.Text} minutes!");
                        return;
                    }
                    Notification.ShowNotificationInfo("orange", "Numbers only are allowed!");
                    return;
                }
                Notification.ShowNotificationInfo("orange", "You must type a vaule greather than 0 !");
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }

        private void StartTimerVaultClose()
        {
            _CloseSession = Globals.sessionExpireInterval * 60;

            _dispatcherTimerLogout = new DispatcherTimer();
            _dispatcherTimerLogout.Tick += UserLogoutTimer!;
            _dispatcherTimerLogout.Interval = new TimeSpan(0, Globals.sessionExpireInterval, 0);
            _dispatcherTimerLogout.Start();

            _dispatcherTimerElapsed = new DispatcherTimer();
            _dispatcherTimerElapsed.Tick += DisplayElapsedTimeUserLogout!;
            _dispatcherTimerElapsed.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimerElapsed.Start();

        }
        private void UserLogoutTimersStop()
        {
            if (_dispatcherTimerLogout != null)
            {
                if (_dispatcherTimerLogout.IsEnabled)
                    _dispatcherTimerLogout.Stop();
                if (_dispatcherTimerElapsed!.IsEnabled)
                    _dispatcherTimerElapsed.Stop();
                vaultExpireTb.Visibility = Visibility.Hidden;
                vaultElapsed.Visibility = Visibility.Hidden;
                _CloseSession = Globals.sessionExpireInterval * 60;
            }
        }
        private void DisplayElapsedTimeUserLogout(object sender, EventArgs e)
        {
            _CloseSession--;

            if (_CloseSession < 60)
            {
                vaultExpireTb.Visibility = Visibility.Visible;
                vaultElapsed.Visibility = Visibility.Visible;
                vaultExpireTb.Text = $"Vault will close in less than a \r\n" +
                    $"minute if no action is made on it!";
                _CloseSession = Globals.sessionExpireInterval * 60;
            }
        }
        private void RestartTimerUserLogout()
        {
            UserLogoutTimersStop();
            StartTimerVaultClose();
        }
        private void UserLogoutTimer(object sender, EventArgs e)
        {
            string[] listOpenWindow = { "UpdateAccountWPF", "AddAccountsWPF", "MasterPasswordWPF", "DelAccountWPF" };
            foreach (var window in listOpenWindow)
            {
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.Name == window)
                    {
                        w.Close();
                    }
                }
            }
            UserManagement.Logout(UsersListVI, AccListVI, SettingsListVI, accList, tabControl, s_masterPassCheckTimer!);
            UserLogoutTimersStop();
        }

        private void Passenger_Closing(object sender, CancelEventArgs e)
        {
            ClipBoardUtil.ClearClipboard(Globals.accountPassword!);
            Globals.accountPassword = string.Empty;
        }
    }
}