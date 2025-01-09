using System.Windows.Controls;

namespace Passenger.Utils
{
    public class TextPassBoxChanges
    {
        public static void TextPassBoxChanged(TextBox application, TextBox account, PasswordBox password, Button button)
        {
            button.IsEnabled = (application.Text.Length > 0 && account.Text.Length > 0 && password.Password.Length > 0);
        }

        public static void ClearTextPassBox(TextBox application, TextBox account, PasswordBox password)
        {
            application.Clear();
            account.Clear();
            password.Clear();
        }

        public static void ShowPassword(PasswordBox passwordBox, TextBox textBox)
        {
            passwordBox.Visibility = System.Windows.Visibility.Collapsed;
            textBox.Visibility = System.Windows.Visibility.Visible;
            textBox.Text = passwordBox.Password;
        }

        public static void HidePassword(PasswordBox passwordBox, TextBox textBox)
        {
            passwordBox.Visibility = System.Windows.Visibility.Visible;
            textBox.Visibility = System.Windows.Visibility.Collapsed;
            textBox.Clear();
        }

        public static void ClearPBoxesInput(PasswordBox password, PasswordBox confirmPassword)
        {
            password.Clear();
            confirmPassword.Clear();
        }

    }
}