using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using Unosquare.RedmineTime.Models.Messages;

namespace Unosquare.RedmineTime.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly RelayCommand _configurationCommand;
        private readonly RelayCommand _helpCommand;

        public MainViewModel()
        {
            _configurationCommand = new RelayCommand(ShowConfiguration);
            _helpCommand = new RelayCommand(OpenHelp);
        }

        private void OpenHelp()
        {
            Messenger.Default.Send(new ShowHelpMessage());
        }

        private void ShowConfiguration()
        {
            Messenger.Default.Send(new EditConfigurationMessage());
        }

        public ICommand ConfigurationCommand => _configurationCommand;

        public ICommand HelpCommand => _helpCommand;

    }
}