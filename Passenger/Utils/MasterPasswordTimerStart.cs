using System.Windows.Threading;

namespace Passenger.Utils
{
    public class MasterPasswordTimerStart
    {
        public static void MasterPasswordCheck_TimerStart(DispatcherTimer masterPasswordTimer)
        {
            masterPasswordTimer.Tick += MasterPasswordCheck_Tick!;
            masterPasswordTimer.Interval = new TimeSpan(0, 30, 0);
            masterPasswordTimer.Start();
        }
        public static void MasterPasswordCheck_TimerStop(DispatcherTimer masterPasswordTimer)
        {
            if (masterPasswordTimer.IsEnabled)
                masterPasswordTimer.Stop();
        }
        private static void MasterPasswordCheck_Tick(object sender, EventArgs e)
        {
            PassengerLib.Globals.masterPasswordCheck = false;
        }
    }
}
