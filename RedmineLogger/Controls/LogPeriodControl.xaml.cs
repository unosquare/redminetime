using GalaSoft.MvvmLight.Messaging;
using RedmineLogger.Models.Messages;
using System.Windows;
using System.Windows.Controls;

namespace RedmineLogger.Controls
{
    /// <summary>
    ///     Interaction logic for LogPeriodControl.xaml
    /// </summary>
    public partial class LogPeriodControl : UserControl
    {
        public LogPeriodControl()
        {
            InitializeComponent();
            Messenger.Default.Register<EditActivityMessage>(this, Edit);

            PeriodListView.ItemContainerGenerator.StatusChanged += (sender, args) =>
            {
                foreach (var item in PeriodListView.Items)
                {
                    var container = PeriodListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                    if (container != null)
                    {
                        container.VerticalContentAlignment = VerticalAlignment.Stretch;
                    }
                }
            };
        }


        private void Edit(EditActivityMessage arguments)
        {
            ActivityDetails.Visibility = Visibility.Visible;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            PeriodListView.ItemTemplate = FindResource("DayActivitySmallTemplate") as DataTemplate;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            PeriodListView.ItemTemplate = FindResource("DayActivityTemplate") as DataTemplate;
        }
    }
}