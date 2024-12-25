using PassengerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;

namespace Passenger.Utils
{
    public class UserManagement
    {
        public static void CreateUser(string userName, string password, string confirmPassword)
        {
            try
            {
                if (Database.isUserExists(userName))
                {
                    Notification.ShowNotificationInfo("orange", "User with this name already exist!");
                    PassengerLib.Globals.vaultChecks = true;
                    return;
                }

                if (userName.Length < 3)
                {
                    Notification.ShowNotificationInfo("orange", "User name must be at least 3 characters long.");
                    PassengerLib.Globals.vaultChecks = true;
                    return;
                }

                if (!PasswordValidator.ValidatePassword(confirmPassword))
                {
                    Notification.ShowNotificationInfo("orange", "Password must be at least 12 characters, and must include at least one upper case letter, one lower case letter, one numeric digit, one special character and no space!");
                    PassengerLib.Globals.vaultChecks = true;
                    return;
                }

                Database.RegisterUser(userName, PasswordValidator.StringToSecureString(password));
                Notification.ShowNotificationInfo("green", $"User {userName} was created!");
                PassengerLib.Globals.createConfirmation = true;
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }

        public static string GetUserNameFromListView(ListView listView)
        {
            string application = string.Empty;
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo("orange", "You must select a user to delete!");
                return application;
            }
            string selectedItem = listView.SelectedItem.ToString()!;
            application = selectedItem.SplitByText(", ", 0).Replace("{ Name = ", string.Empty);
            return application;

        }

        private static void DeleteUser(string userName, ListView listView)
        {

        }

        public static void ListUsers(ListView listView)
        {
            PassengerLib.Globals.vaultsCount = 0;
            listView.Items.Clear();

            var usersList = Database.GetUserList();
            foreach (var user in usersList)
            {
                PassengerLib.Globals.vaultsCount++;
                listView.Items.Add(new { Name = user.Name, CreateDate = user.DateCreated });
            }
        }

        public static void Logout(ListViewItem userListView, ListViewItem accountListView, ListViewItem settingsListView,
                                      ListView accountList, TabControl tabControl, DispatcherTimer masterPasswordTimer)

        {
            ListViewSettings.SetListViewColor(userListView, false);
            ListViewSettings.SetListViewColorApp(accountListView, true);
            ListViewSettings.SetListViewColor(settingsListView, true);
            accountList.Items.Clear();
            tabControl.SelectedIndex = 0;
            accountListView.Foreground = Brushes.Red;
            accountListView.IsEnabled = false;
            Globals.masterPassword = null;
            Globals.vaultOpen = false;
            
            //MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(masterPasswordTimer);
            GC.Collect();
        }


        public static void ChangeMasterPassword(ListView userList)
        {

        }

    }
}
