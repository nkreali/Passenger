using PassengerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Security.Principal;

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
            
            User? selectedUser = (User)listView.SelectedItem;
            application = selectedUser.Name!;

            return application;

        }

        public static void DeleteUserItem(ListView listView)
        {
            string userName = ((User)listView.SelectedItem).Name!;
            PassengerLib.Globals.vaultName = userName;

            DeleteUser deleteUser = new DeleteUser();
            deleteUser.ShowDialog();
            if (PassengerLib.Globals.deleteConfirmation)
            {
                DeleteUser(userName, listView);
                ClearVariables.VariablesClear();
            }
        }
        private static void DeleteUser(string userName, ListView usersList)
        {
            try
            {
                int id = Database.GetUserID(userName);
                Database.DeleteUser(id);
                Notification.ShowNotificationInfo("green", $"User {userName} was removed from list!");
                ListUsers(usersList);
            }
            catch (Exception e)
            {
                Notification.ShowNotificationInfo("red", e.Message);
            }

        }

        public static void ListUsers(ListView listView)
        {
            PassengerLib.Globals.vaultsCount = 0;
            listView.ItemsSource = null;

            var usersList = Database.GetUserList();
            listView.ItemsSource = usersList;
            //foreach (var user in usersList)
            //{
            //    PassengerLib.Globals.vaultsCount++;
            //    listView.Items.Add(new { Name = user.Name, DateCreated = user.DateCreated });
            //}
        }

        public static void Logout(ListViewItem userListView, ListViewItem accountListView, ListViewItem settingsListView,
                                      ListView accountList, TabControl tabControl, DispatcherTimer masterPasswordTimer)

        {
            ListViewSettings.SetListViewColor(userListView, false);
            ListViewSettings.SetListViewColorApp(accountListView, true);
            ListViewSettings.SetListViewColor(settingsListView, true);
            accountList.ItemsSource = null;
            
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
            string oldMasterPassword = PasswordValidator.ConvertSecureStringToString(PassengerLib.Globals.masterPassword!);
            string newMasterPassword = PasswordValidator.ConvertSecureStringToString(PassengerLib.Globals.newMasterPassword!);
            if (userList.SelectedItem == null)
            {
                Notification.ShowNotificationInfo("orange", "You must select a vault for changeing the Master Password!");
                return;
            }

            string userName = GetUserNameFromListView(userList);
            if (!Database.isUserExists(userName))
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                return;
            }

            int id = Database.GetUserID(userName);
            List<Account> accounts = Database.GetAccountsList(id);
            accounts[0].IsPasswordVisible = true;
            if (AES.Decrypt(accounts[0].Password, oldMasterPassword).Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo("red", "Incorrect master password!");
                return;
            }
            foreach (var account in accounts)
            {
                account.IsPasswordVisible = true;
                account.Password = AES.Decrypt(account.Password!, oldMasterPassword);
                account.Password = AES.Encrypt(account.Password!, newMasterPassword);
                Database.UpdateAccount(account, account.Password);
            }
            //if (accounts[0].Password!.Contains("Error decrypting"))
            //{
            //    Notification.ShowNotificationInfo("red", "Incorrect master password!");
            //    return;
            //}

            //foreach (var account in accounts)
            //{
            //    account.Password = AES.Encrypt(account.Password!, newMasterPassword);
            //    Database.UpdateAccount(account, account.Password);
            //}
            Database.UpdateUsersPassword(userName, PassengerLib.Globals.newMasterPassword!);
                        
            Notification.ShowNotificationInfo("green", $"New Master Password was set for user {userName}!");
        }

    }
}
