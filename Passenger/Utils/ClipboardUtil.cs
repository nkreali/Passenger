using System.Windows;

namespace Passenger.Utils
{
    public class ClipBoardUtil
    {
        public static void ClearClipboard(string accPassword)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();

                    if (clipboardText == accPassword)
                    {
                        Clipboard.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }
    }
}
