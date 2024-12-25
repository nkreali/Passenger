using System.Runtime.Versioning;
using System.Security;

namespace Passenger.Utils
{
    public class MasterPasswordLoad
    {
        public static SecureString LoadMasterPassword(string userName)
        {
            SecureString password;
            PassengerLib.Globals.vaultName = userName;
            MasterPassword masterPassword = new MasterPassword();
            masterPassword.ShowDialog();
            password = masterPassword.masterPassword!;
            masterPassword.masterPasswordPWD.Clear();
            masterPassword.masterPassword = null;
            return password;
        }
    }
}
