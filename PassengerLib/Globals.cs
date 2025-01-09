using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace PassengerLib
{
    public static class Globals
    {
        public static SecureString? masterPassword { get; set; }
        public static SecureString? newMasterPassword { get; set; }
        public static string? serviceName { get; set; }
        public static string? accountName { get; set; }
        public static string? accountPassword { get; set; }
        public static string? newAccountPassword { get; set; }
        public static bool deleteConfirmation = false;
        public static bool createConfirmation = false;
        public static bool updatePwdConfirmation = false;
        public static bool closeAppConfirmation = false;
        public static string? userName { get; set; }
        public static string? gridColor { get; set; }
        public static string? messageData { get; set; }
        public static bool userChecks = false;
        public static bool vaultOpen = false;
        public static bool sharedVault = false;
        public static bool masterPasswordCheck = true;
        public static readonly string registryPath = "SOFTWARE\\Passenger";
        public static readonly string userExpireReg = "UserExpireSession";
        public static int sessionExpireInterval { get; set; }
        
    }
}
