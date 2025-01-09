using System.Runtime.Versioning;
using PassengerLib;
using Passenger.Model;

namespace Passenger.Utils
{
    public class Notification
    {
        public static void ShowNotificationInfo(string gridColor, string messageData)
        {
            Globals.gridColor = gridColor;
            Globals.messageData = messageData;
            PopMessage popMessage = new PopMessage();
            popMessage.ShowDialog();
        }
    }
}