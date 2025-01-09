using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Passenger.Utils
{
    public class ListViewSettings
    {
        public static void SetListViewColor(ListViewItem listViewItem, bool reset)
        {
            var converter = new BrushConverter();
            if (reset)
            {
                listViewItem.Background = Brushes.Transparent;
                listViewItem.Foreground = (Brush?)converter.ConvertFromString("#FFDCDCDC");
                return;
            }
            listViewItem.Background = (Brush?)converter.ConvertFromString("#4a3e8e");
        }
        public static void SetListViewColorApp(ListViewItem listViewItem, bool reset)
        {
            if (listViewItem.IsEnabled)
            {
                var converter = new BrushConverter();
                if (reset)
                {
                    listViewItem.Background = Brushes.Transparent;
                    listViewItem.Foreground = (Brush?)converter.ConvertFromString("#FFDCDCDC");
                    return;
                }
                listViewItem.Background = (Brush?)converter.ConvertFromString("#4a3e8e");
            }
        }
        public static void ListViewSortSetting(ListView listView, string columnName, bool liveSort)
        {
            listView.Items.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
            listView.Items.IsLiveSorting = liveSort;
            listView.Items.LiveSortingProperties.Add(columnName);
        }
    }
}
