using System.Windows;
using System.Windows.Media;
using PassengerLib;

namespace Passenger
{
    public partial class PopMessage : Window
    {
        public PopMessage()
        {
            InitializeComponent();
            SetUI(Globals.gridColor!, Globals.messageData!);

        }

        private void ConfirmBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetUI(string gridColor, string messageData)
        {
            switch (gridColor)
            {
                case "green":
                    popGrid.Background = Brushes.Green;
                    titleTxt.Text = "Notification";
                    NotificationLBL.Text = messageData;
                    break;

                case "red":
                    popGrid.Background = Brushes.Red;
                    titleTxt.Text = "ERROR";
                    NotificationLBL.Text = messageData;
                    break;

                case "orange":
                    popGrid.Background = Brushes.DarkOrange;
                    titleTxt.Text = "WARNING";
                    NotificationLBL.Text = messageData;
                    break;
            }
        }
    }
}
