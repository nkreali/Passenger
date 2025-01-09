using Passenger.Utils;
using System;
using System.Runtime.Versioning;
using System.Windows.Controls;

namespace Passenger.Utils
{
    public class UserSessionExpire
    {
        public static void LoadExpireTime(string registryPath, string key, string keyValue, TextBox periodBox)
        {
            int expireTime = Int32.Parse(keyValue);
            try
            {
                string value = RegistryManagement.RegKey_Read("HKEY_CURRENT_USER\\" + registryPath, key);
                if (!string.IsNullOrEmpty(value))
                {
                    expireTime = Int32.Parse(value);
                    if (expireTime >= 1)
                    {
                        periodBox.Text = value;
                        PassengerLib.Globals.sessionExpireInterval = expireTime;
                        return;
                    }
                    return;
                }
                RegistryManagement.RegKey_CreateKey(registryPath, key, keyValue);
                PassengerLib.Globals.sessionExpireInterval = expireTime;
                periodBox.Text = keyValue;
                return;
            }
            catch
            {
                RegistryManagement.RegKey_CreateKey(registryPath, key, keyValue);
                PassengerLib.Globals.sessionExpireInterval = expireTime;
                periodBox.Text = keyValue;
                Notification.ShowNotificationInfo("red", "The user session expire time could not be read due to an error. Expire time was set to default value of 10 minutes!");
            }
        }
    }
}
