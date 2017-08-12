using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using Unosquare.RedmineTime.Controls;
using Unosquare.RedmineTime.Helpers;
using Unosquare.RedmineTime.Models;
using Unosquare.RedmineTime.Models.Messages;

namespace Unosquare.RedmineTime
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool _closeNow;

        private decimal _unsavedHours;

        public MainWindow()
        {
            Messenger.Default.Register<NotificationMessageAction>(this, ShowConfirmation);
            Messenger.Default.Register<NotificationMessage>(this, ShowNotification);
            Messenger.Default.Register<SelectProjectMessage>(this, message => NavigateToSelectProject());
            Messenger.Default.Register<LogTimeMessage>(this, message => NavigateToLogTime());
            Messenger.Default.Register<EditConfigurationMessage>(this, message => NavigateToConfiguration());
            Messenger.Default.Register<UnsavedHoursReportMessage>(this, UpdateUnsavedHours);
            Messenger.Default.Register<ShowHelpMessage>(this, ShowHelpWindow);

            InitializeComponent();

            RedmineService redmineService = null;
            var configuration = new LoggerConfiguration();
            if (!string.IsNullOrWhiteSpace(configuration.RedmineUrl) &&
                !string.IsNullOrWhiteSpace(configuration.RedmineApiKey))
            {
                try
                {
                    redmineService = RedmineService.ConnectAndCreate(configuration);
                }
                catch (Exception)
                {
                    redmineService = null;
                }
            }

            if (redmineService != null)
            {
                Messenger.Default.Send(new RedmineConnectionSuccessMessage(redmineService));
                NavigateToSelectProject();
            }
            else
            {
                NavigateToConfiguration();
            }
        }

        private void ShowHelpWindow(ShowHelpMessage obj)
        {
            var helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        private void UpdateUnsavedHours(UnsavedHoursReportMessage message)
        {
            _unsavedHours = message.UnsavedHours;
        }

        private async void ShowNotification(NotificationMessage message)
        {
            await this.ShowMessageAsync("Error", message.Notification);
        }

        private async void ShowConfirmation(NotificationMessageAction message)
        {
            var result = await
                this.ShowMessageAsync("Confirmation",
                    message.Notification,
                    MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                await Dispatcher.BeginInvoke(new Action(message.Execute));
            }
        }

        private async void CanLoseUnsavedEntries(Action action)
        {
            if (_unsavedHours == 0)
            {
                await Dispatcher.BeginInvoke(action);
                return;
            }

            var result = await
                this.ShowMessageAsync("Unsaved entries",
                    "There are unsaved time entries. Are you sure you want to lose them?",
                    MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                await Dispatcher.BeginInvoke(action);
            }
        }

        private void NavigateToLogTime()
        {
            _unsavedHours = 0;
            Content = new LogPeriodControl();
            EditConfigurationButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
        }

        private void NavigateToSelectProject()
        {
            CanLoseUnsavedEntries(() =>
            {
                _unsavedHours = 0;
                Content = new ProjectSelectionControl();
                EditConfigurationButton.Visibility = Visibility.Visible;
                HelpButton.Visibility = Visibility.Hidden;
            });
        }

        private void NavigateToConfiguration()
        {
            CanLoseUnsavedEntries(() =>
            {
                _unsavedHours = 0;
                Content = new ConfigurationControl();
                EditConfigurationButton.Visibility = Visibility.Hidden;
                HelpButton.Visibility = Visibility.Hidden;
            });
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (_closeNow && _unsavedHours <= 0) return;
            e.Cancel = true;
            CanLoseUnsavedEntries(() =>
            {
                _closeNow = true;
                Application.Current.Shutdown(0);
            });
        }
    }
}