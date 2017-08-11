using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;
using RedmineLogger.Models;
using RedmineLogger.Models.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RedmineLogger.ViewModel
{
    public class LogPeriodViewModel : ViewModelBase
    {
        private readonly RelayCommand _addTimeEntryCommand;
        private readonly RelayCommand _returnToProjectSelectionCommand;
        private readonly RelayCommand _saveEntriesCommand;
        private ObservableCollection<DayActivity> _days;
        private WorkPeriod _period;
        private decimal _periodTotalHours;
        private decimal _periodUnsavedHours;
        private int _savingProgress;
        private DayActivity _selectedDay;
        private User _selectedUser;
        private RedmineUserProject _selectedUserProject;

        public LogPeriodViewModel()
        {
            Messenger.Default.Register<LogTimeMessage>(this, Initialize);
            Messenger.Default.Register<AddNewTimeEntryMessage>(this, args => AddNewEntry(args.Entry));
            Messenger.Default.Register<RemoveEntryMessage>(this, args => RemoveEntry(args.TimeEntry));
            _saveEntriesCommand = new RelayCommand(SaveEntries, CanSaveEntries);
            _returnToProjectSelectionCommand = new RelayCommand(ReturnToProjectSelection);
            _addTimeEntryCommand = new RelayCommand(AddNewEntry);
        }

        public WorkPeriod Period
        {
            get { return _period; }
            set
            {
                Set(() => Period, ref _period, value);
                InitializeDays();
            }
        }

        public ICommand SaveEntriesCommand => _saveEntriesCommand;

        public ICommand AddTimeEntryCommand => _addTimeEntryCommand;

        public decimal PeriodUnsavedHours
        {
            get { return _periodUnsavedHours; }
            set
            {
                Set(() => PeriodUnsavedHours, ref _periodUnsavedHours, value);
                // Notify of unsaved hours
                Messenger.Default.Send(new UnsavedHoursReportMessage(_periodUnsavedHours));
            }
        }

        public decimal PeriodTotalHours
        {
            get { return _periodTotalHours; }
            set { Set(() => PeriodTotalHours, ref _periodTotalHours, value); }
        }

        public RedmineUserProject SelectedUserProject
        {
            get { return _selectedUserProject; }
            set { Set(() => SelectedUserProject, ref _selectedUserProject, value); }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set { Set(() => SelectedUser, ref _selectedUser, value); }
        }

        public ObservableCollection<DayActivity> Days
        {
            get { return _days ?? (_days = new ObservableCollection<DayActivity>()); }
            set { Set(() => Days, ref _days, value); }
        }

        public DayActivity SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                Set(() => SelectedDay, ref _selectedDay, value);
                Messenger.Default.Send(new SelectedDayMessage(SelectedDay.Day));
            }
        }

        public int SavingProgress
        {
            get { return _savingProgress; }
            set { Set(() => SavingProgress, ref _savingProgress, value); }
        }

        public ICommand ReturnToProjectSelectionCommand => _returnToProjectSelectionCommand;

        private void AddNewEntry()
        {
            var day = Days.FirstOrDefault(d => d.TotalHours < 8) ?? Days.First();
            Messenger.Default.Send(new EditActivityMessage(day.Day, _selectedUser, _selectedUserProject, _period, true));
        }

        private void ReturnToProjectSelection()
        {
            Messenger.Default.Send(new SelectProjectMessage());
        }

        private void RemoveEntry(RedmineTimeEntry entry)
        {
            var entryDay = entry?.TimeEntryInfo?.SpentOn;
            if (entryDay == null)
                throw new InvalidOperationException("Invalid Entry to remove");
            var day = Days.SingleOrDefault(d => d.Day.Date == entryDay.Value.Date);
            if (day == null)
                throw new InvalidOperationException("Day of Entry to be removed couldn't be found");
            day.TimeEntries.Remove(entry);
        }

        private bool CanSaveEntries()
        {
            return
                Days.SelectMany(d => d.TimeEntries)
                    .Any(t => t.Origin == TimeEntryOrigin.NewInLogger || t.Origin == TimeEntryOrigin.Outlook);
        }

        private async void SaveEntries()
        {
            var entries =
                Days.SelectMany(d => d.TimeEntries)
                    .Where(t => t.Origin == TimeEntryOrigin.NewInLogger || t.Origin == TimeEntryOrigin.Outlook)
                    .ToArray();
            SavingProgress = 0;
            for (var i = 0; i < entries.Length; i++)
            {
                await SelectedUserProject.LogTime(entries[i]);
                SavingProgress = i*100/entries.Length;
            }
            SavingProgress = 0;
            InitializeDays();
        }

        private void AddNewEntry(RedmineTimeEntry entry)
        {
            var day = Days.FirstOrDefault(d => d.Day.Date == entry?.TimeEntryInfo?.SpentOn?.Date);
            day?.TimeEntries.Add(entry);
            RefreshPeriodTotalHours();
            _saveEntriesCommand.RaiseCanExecuteChanged();
        }

        private void InitializeDays()
        {
            InitializeDaysFromPeriod();
            GetSavedActivities();
        }

        private void Initialize(LogTimeMessage arguments)
        {
            SelectedUser = arguments.User;
            SelectedUserProject = arguments.UserProject;
            Period = arguments.Period;
        }

        private void GetSavedActivities()
        {
            var currentEntries = SelectedUserProject.GetTimeEntries(Period);
            foreach (var entry in currentEntries)
            {
                var day = Days.FirstOrDefault(d => entry.SpentOn != null && d.Day.Date == entry.SpentOn.Value.Date);
                day?.TimeEntries.Add(new RedmineTimeEntry(TimeEntryOrigin.RedmineService) {TimeEntryInfo = entry});
            }
            RefreshPeriodTotalHours();
        }

        private void RefreshPeriodTotalHours()
        {
            PeriodTotalHours = Days.Sum(d => d.TotalHours);
            PeriodUnsavedHours = Days.Sum(d => d.UnsavedHours);
        }

        private void InitializeDaysFromPeriod()
        {
            Days.Clear();
            foreach (var day in Period.EachDay)
            {
                var dayType = day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday
                    ? WorkableDayType.WeekendDay
                    : WorkableDayType.WorkingDay;
                Days.Add(new DayActivity(day, SelectedUser, SelectedUserProject, Period, dayType));
            }
        }

        public override void Cleanup()
        {
            Messenger.Default.Unregister<LogTimeMessage>(this, Initialize);
            Messenger.Default.Unregister<AddNewTimeEntryMessage>(this, args => AddNewEntry(args.Entry));
            Messenger.Default.Unregister<RemoveEntryMessage>(this, args => RemoveEntry(args.TimeEntry));
            base.Cleanup();
        }
    }
}