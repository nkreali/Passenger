using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using PassengerLib;
using System.Windows.Controls;
using System.Text.Json;
using System.Security.Principal;
using System.IO;

namespace Passenger.Utils
{
    public class AccountManagement
    {
        public static SecureString? vaultSecure = null;

        public static bool DecryptAndPopulateList(ListView listView, string userName, SecureString masterPassword)
        {
            try
            {
                listView.ItemsSource = null;

                if (!Database.isUserExists(userName))
                {
                    Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                    return false;
                }

                if (masterPassword == null)
                {
                    Notification.ShowNotificationInfo("red", "Master password must be entered!");
                    return false;
                }

                var list = Database.GetAccountsList(Database.GetUserID(userName));
                foreach (var account in list)
                {
                    account.IsPasswordVisible = true;
                    account.Password = AES.Decrypt(account.Password!, PasswordValidator.ConvertSecureStringToString(masterPassword));
                    if (account.Password.Contains("Error decrypting"))
                    {
                        Notification.ShowNotificationInfo("red", "Incorrect master password!");
                        Globals.masterPasswordCheck = false;
                        MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer!);
                        return false;
                    }
                    account.IsPasswordVisible = false;

                }
                listView.ItemsSource = list;

                return true;
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
                return false;
            }
        }

        public static void AddAccount(ListView listView, string userName, string service,
                                        string login, string accountPassword, SecureString masterPassword)
        {
            if (!Database.isUserExists(userName))
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                return;
            }
            if (masterPassword == null)
            {
                ClearVariables.VariablesClear();
                return;
            }

            Account account = new Account
            {
                Owner_Id = Database.GetUserID(userName),
                Service = service,
                Login = login,
                Password = accountPassword,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
            };

            if (Database.AccountExists(account.Owner_Id, account))
            {
                Notification.ShowNotificationInfo("orange", $"Service {account.Service} already contains {account.Login} account!");
                return;
            }

            if (login.Length < 3)
            {
                Notification.ShowNotificationInfo("orange", "The length of login should be at least 3 characters!");
                return;
            }

            var test_acc = Database.GetAccountsList(account.Owner_Id)[0];
            test_acc.IsPasswordVisible = true;
            test_acc.Password = AES.Decrypt(test_acc.Password!, PasswordValidator.ConvertSecureStringToString(masterPassword));
            if (test_acc.Password.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo("red", "Incorrect master password!");
                PassengerLib.Globals.masterPasswordCheck = false;
                MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer!);
                return;
            }

            account.IsPasswordVisible = true;
            account.Password = AES.Encrypt(account.Password, PasswordValidator.ConvertSecureStringToString(masterPassword));
            if (Database.isUserExists(userName))
            {
                try
                {
                    Database.AddAccount(account.Owner_Id, account);
                    Notification.ShowNotificationInfo("green", $"Data for {service} is encrypted and added to vault!");
                    account.IsPasswordVisible = false;
                    DecryptAndPopulateList(listView, userName, masterPassword);
                    return;
                }
                catch (Exception ex)
                {
                    Notification.ShowNotificationInfo("red", ex.Message);
                }
            }
            else
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                ListViewSettings.ListViewSortSetting(listView, "site/application", false);
            }
            account.IsPasswordVisible = false;
            listView.ItemsSource = null;
            DecryptAndPopulateList(listView, userName, masterPassword);
        }

        public static void UpdateAccount(ListView listView, string userName, string service,
                                string login, string accountPassword, SecureString masterPassword)
        {
            if (masterPassword == null)
            {
                ClearVariables.VariablesClear();
                return;
            }
            if (!Database.isUserExists(userName))
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                return;
            }

            Account acc = Database.GetAccount(Database.GetUserID(userName), login, service);
            if (acc == null)
            {
                Notification.ShowNotificationInfo("orange", $"This account does not exist!");
                return;
            }
            acc.IsPasswordVisible = true;
            acc.Password = AES.Decrypt(acc.Password!, PasswordValidator.ConvertSecureStringToString(masterPassword));
            if (acc.Password.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo("red", "Incorrect master password!");
                PassengerLib.Globals.masterPasswordCheck = false;
                MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer!);
                return;
            }

            if (Database.isUserExists(userName))
            {
                try
                {
                    accountPassword = AES.Encrypt(accountPassword, PasswordValidator.ConvertSecureStringToString(masterPassword));
                    Database.UpdateAccount(acc, accountPassword);
                    Notification.ShowNotificationInfo("green", $"Password for account {acc.Login} for {acc.Service} was updated!");
                    DecryptAndPopulateList(listView, userName, masterPassword);
                    return;
                }
                catch (Exception ex)
                {
                    Notification.ShowNotificationInfo("red", ex.Message);
                }
            }
            else
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                ListViewSettings.ListViewSortSetting(listView, "site/application", false);
            }
            DecryptAndPopulateList(listView, userName, masterPassword);
        }
        public static void UpdateSelectedItemPassword(ListView listView, string userName)
        {
            Account acc = (Account)listView.SelectedItem;
            if (acc != null)
            {
                acc.IsPasswordVisible = true;
                string? service = acc.Service;
                string? login = acc.Login;
                PassengerLib.Globals.accountName = login;
                PassengerLib.Globals.serviceName = service;
                UpdateAccount updateAcc = new UpdateAccount();
                updateAcc.ShowDialog();
                string? newPassword = PassengerLib.Globals.newAccountPassword;
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!PassengerLib.Globals.masterPasswordCheck)
                    {
                        var masterPassword = MasterPasswordLoad.LoadMasterPassword(userName);
                        UpdateAccount(listView, userName, service!, login!, newPassword, masterPassword);
                        ClearVariables.VariablesClear();
                        return;
                    }
                    UpdateAccount(listView, userName, service!, login!, newPassword, PassengerLib.Globals.masterPassword!);
                    ClearVariables.VariablesClear();
                }
            }
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }
        public static void DeleteAccount(ListView listView, string userName, string service,
                                        string login, SecureString masterPassword)
        {            
            if (masterPassword == null)
            {
                ClearVariables.VariablesClear();
                return;
            }
            if (!Database.isUserExists(userName))
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                return;
            }

            Account acc = Database.GetAccount(Database.GetUserID(userName), login, service);
            if (acc == null)
            {
                Notification.ShowNotificationInfo("orange", $"This account does not exist!");
                return;
            }
            acc.IsPasswordVisible = true;
            acc.Password = AES.Decrypt(acc.Password!, PasswordValidator.ConvertSecureStringToString(masterPassword));
            if (acc.Password.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo("red", "Incorrect master password!");
                PassengerLib.Globals.masterPasswordCheck = false;
                MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer!);
                return;
            }

            if (Database.isUserExists(userName))
            {
                try
                {
                    Database.DeleteAccount(acc);
                    Notification.ShowNotificationInfo("green", $"Account {acc.Login} for {acc.Service} was deleted!");
                    DecryptAndPopulateList(listView, userName, masterPassword);
                    return;
                }
                catch (Exception ex)
                {
                    Notification.ShowNotificationInfo("red", ex.Message);
                }
            }
            else
            {
                Notification.ShowNotificationInfo("red", $"User {userName} does not exist!");
                ListViewSettings.ListViewSortSetting(listView, "site/application", false);
            }
            listView.ItemsSource = null;
            DecryptAndPopulateList(listView, userName, masterPassword);
        }


        public static void ShowPassword(ListView listView)
        {
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo("orange", "You must select an application line to show the account password!");
                return;
            }

            Account? selectedItem = (Account)listView.SelectedItem;
            if (selectedItem.IsPasswordVisible == false)
            {
                selectedItem.IsPasswordVisible = true;
            }
            else
            {
                selectedItem.IsPasswordVisible = false;
            }
        }

        public static string CopyPassToClipBoard(ListView listView)
        {
            string outPass = string.Empty;
            Account acc = (Account)listView.SelectedItem;
            if (acc != null)
            {
                acc.IsPasswordVisible = true;
                PassengerLib.Globals.accountPassword = acc.Password;
                outPass = acc.Password!;
                acc.IsPasswordVisible = false;
                Notification.ShowNotificationInfo("green", $"Password for {acc.Login} is copied to clipboard!");
            }
            
            return outPass;
        }
        public static void DeleteSelectedItem(ListView listView, string userName, ListView userList)
        {

            Account account = (Account)listView.SelectedItem;
            if (account != null)
            {
                PassengerLib.Globals.accountName = account.Login;
                PassengerLib.Globals.serviceName = account.Service;
                DeleteAccount delAcc = new DeleteAccount();
                delAcc.ShowDialog();
                if (PassengerLib.Globals.deleteConfirmation)
                {
                    if (!PassengerLib.Globals.masterPasswordCheck)
                    {
                        var masterPassword = MasterPasswordLoad.LoadMasterPassword(userName);
                        DeleteAccount(listView, userName, account.Service!, account.Login!, masterPassword);
                        ClearVariables.VariablesClear();
                        return;
                    }
                    DeleteAccount(listView, userName, account.Service!, account.Login!, PassengerLib.Globals.masterPassword!);
                    ClearVariables.VariablesClear();
                }
            }
            else
            {
                Notification.ShowNotificationInfo("orange", "You must select an application to delete!");
            }
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }
    } 
}

