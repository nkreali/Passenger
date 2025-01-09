using Microsoft.Win32;

namespace Passenger.Utils
{
    public static class RegistryManagement
    {
        public static void RegKey_WriteSubkey(string keyName, string subKeyName, string subKeyValue)
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(keyName, true);
            rk!.SetValue(subKeyName, subKeyValue);
        }
        public static string RegKey_Read(string keyName, string subKeyName)
        {
            string key = string.Empty;

            string InstallPath = (string)Registry.GetValue(keyName, subKeyName, null)!;
            if (InstallPath != null)
            {
                key = InstallPath;
            }
            return key;
        }

        public static void RegKey_CreateKey(string keyName, string subKeyName, string subKeyValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey
            (keyName);

            key.SetValue(subKeyName, subKeyValue);
            key.Close();
        }
    }
}