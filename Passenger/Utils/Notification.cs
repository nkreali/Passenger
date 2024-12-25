using System.Runtime.Versioning;
using PassengerLib;
using Passenger.Model;

namespace Passenger.Utils
{
    public class Notification
    {
        [SupportedOSPlatform("Windows")]
        /// <summary>
        /// Show notificaiton pop up message box with diferent case color.
        /// </summary>
        /// <param name="gridColor">red - Error| green -  Confirmation | orange - Warning</param>
        /// <param name="messageData"></param>
        public static void ShowNotificationInfo(string gridColor, string messageData)
        {
            Globals.gridColor = gridColor;
            Globals.messageData = messageData;
            PopMessage popMessage = new PopMessage();
            popMessage.ShowDialog();
        }
    }
}