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

namespace Passenger.Utils
{
    public class AccountManagement
    {
        public static SecureString? vaultSecure = null;

        public static bool DecryptAndPopulateList(ListView listView, string userName, SecureString masterPassword)
        {
            try
            {
                listView.Items.Clear();

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
                        //MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer);
                        return false;
                    }
                    account.IsPasswordVisible = false;
                    //listView.Items.Add(new
                    //{
                    //    Service = account.Service,
                    //    Login = account.Login,
                    //    Password = passMask,
                    //    DateCreated = account.DateCreated,
                    //    DateModified = account.DateModified
                    //});

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

            /*string decryptVault = AES.Decrypt(readVault, PasswordValidator.ConvertSecureStringToString(masterPassword));
            if (decryptVault.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo("red", "Something went wrong. Master password is incorrect or vault issue!");
                PwMLib.GlobalVariables.masterPasswordCheck = false;
                MasterPasswordTimerStart.MasterPasswordCheck_TimerStop(MainWindow.s_masterPassCheckTimer);
                return;
            }*/
            if (login.Length < 3)
            {
                Notification.ShowNotificationInfo("orange", "The length of login should be at least 3 characters!");
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
        }

        public static void AddAccsToTempList(ListView listView)
        {
            PassengerLib.Globals.listItems.Clear();
            for (int i = 0; i <= listView.Items.Count - 1; i++)
                PassengerLib.Globals.listItems.Add(listView.Items[i].ToString()!);
        }
        public static void UpdateAccount(ListView listView, string userName, string service,
                                string login, string accountPassword, SecureString masterPassword)
        {

        }
        public static void DeleteAccount(ListView listView, string userName, string service,
                                        string login, SecureString masterPassword)
        {

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

    } 
}

