using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;
using RedmineLogger.Models.Messages;
using System.Windows.Input;

namespace RedmineLogger.Models
{
    public class RedmineTimeEntry: ViewModelBase
    {

        private TimeEntry _timeEntryInfo;
        private bool _isModified;
        private readonly RelayCommand _removeEntryCommand;

        public RedmineTimeEntry(TimeEntryOrigin origin, TimeEntry timeEntryInfo = null)
        {
            _timeEntryInfo = timeEntryInfo;
            IsModified = false;
            Origin = origin;
            _removeEntryCommand = new RelayCommand(Remove);
        }

        private void Remove()
        {
            Messenger.Default.Send(new RemoveEntryMessage(this));
        }

        public TimeEntry TimeEntryInfo
        {
            get { return _timeEntryInfo; }
            set { Set(() => TimeEntryInfo, ref _timeEntryInfo, value); }
        }

        public TimeEntryOrigin Origin { get; }

        public bool CanBeRemoved => Origin != TimeEntryOrigin.RedmineService;

        public bool IsModified
        {
            get { return _isModified; }
            set { Set(() => IsModified, ref _isModified, value); }
        }

        public ICommand RemoveEntryCommand => _removeEntryCommand;

    }
}