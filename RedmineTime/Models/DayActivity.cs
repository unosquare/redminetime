using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using Redmine.Net.Api.Types;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Unosquare.RedmineTime.Models.Messages;

namespace Unosquare.RedmineTime.Models
{
    public class DayActivity : ViewModelBase, IDropTarget
    {
        private readonly RelayCommand _addTimeEntryCommand;
        private readonly WorkPeriod _period;
        private readonly RedmineUserProject _project;
        private readonly RelayCommand _setHolidayCommand;
        private readonly RelayCommand _setPersonalTimeOffDayCommand;
        private readonly RelayCommand _setWeekendDayCommand;
        private readonly RelayCommand _setWorkingDayCommand;
        private readonly User _user;
        private DateTime _day;
        private RedmineTimeEntry _selectedTimeEntry;
        private ObservableCollection<RedmineTimeEntry> _timeEntries;
        private decimal _totalHours;
        private WorkableDayType _workableDayType;
        private decimal _unsavedHours;

        public DayActivity()
        {
            TotalHours = 0;
            UnsavedHours = 0;
            TimeEntries = new ObservableCollection<RedmineTimeEntry>();
            _addTimeEntryCommand = new RelayCommand(AddTimeEntry, CanAddTimeEntry);
            _setHolidayCommand = new RelayCommand(SetAsHoliday, CanSetAsHoliday);
            _setWorkingDayCommand = new RelayCommand(SetAsWorkingDay, CanSetAsWorkingDay);
            _setWeekendDayCommand = new RelayCommand(SetAsWeekendDay, CanSetAsWeekendDay);
            _setPersonalTimeOffDayCommand = new RelayCommand(SetAsPto, CanSetAsPto);
        }

        public DayActivity(DateTime day, User user, RedmineUserProject project, WorkPeriod period,
            WorkableDayType workableDayType) : this()
        {
            _user = user;
            _project = project;
            _period = period;
            Day = day;
            WorkableDayType = workableDayType;
        }

        public DateTime Day
        {
            get { return _day; }
            set { Set(() => Day, ref _day, value); }
        }

        public ICommand AddTimeEntryCommand => _addTimeEntryCommand;
        public ICommand SetHolidayCommand => _setHolidayCommand;
        public ICommand SetWorkingDayCommand => _setWorkingDayCommand;
        public ICommand SetPersonalTimeOffDayCommand => _setPersonalTimeOffDayCommand;
        public ICommand SetWeekendDayCommand => _setWeekendDayCommand;


        public ObservableCollection<RedmineTimeEntry> TimeEntries
        {
            get { return _timeEntries; }
            private set
            {
                Set(() => TimeEntries, ref _timeEntries, value);
                TimeEntries.CollectionChanged += (sender, args) => { RefreshHours(); };
            }
        }

        public RedmineTimeEntry SelectedTimeEntry
        {
            get { return _selectedTimeEntry; }
            set
            {
                Set(() => SelectedTimeEntry, ref _selectedTimeEntry, value);
                Messenger.Default.Send(new TimeEntrySelectedMessage(SelectedTimeEntry));
            }
        }

        public decimal TotalHours
        {
            get { return _totalHours; }
            set { Set(() => TotalHours, ref _totalHours, value); }
        }
        public decimal UnsavedHours
        {
            get { return _unsavedHours; }
            set { Set(() => UnsavedHours, ref _unsavedHours, value); }
        }


        public WorkableDayType WorkableDayType
        {
            get { return _workableDayType; }
            set
            {
                Set(() => WorkableDayType, ref _workableDayType, value);
                WorkableDayTypeChanged();
            }
        }

        private void SetAsWeekendDay()
        {
            WorkableDayType = WorkableDayType.WeekendDay;
        }

        private bool CanSetAsWeekendDay()
        {
            return WorkableDayType != WorkableDayType.WeekendDay && (Day.DayOfWeek == DayOfWeek.Saturday ||
                                                                     Day.DayOfWeek == DayOfWeek.Sunday);
        }

        private void WorkableDayTypeChanged()
        {
            _setHolidayCommand.RaiseCanExecuteChanged();
            _setPersonalTimeOffDayCommand.RaiseCanExecuteChanged();
            _setWorkingDayCommand.RaiseCanExecuteChanged();
            _setWeekendDayCommand.RaiseCanExecuteChanged();
        }

        private bool CanSetAsPto()
        {
            return WorkableDayType != WorkableDayType.PersonalTimeOff;
        }

        private void SetAsPto()
        {
            WorkableDayType = WorkableDayType.PersonalTimeOff;
        }

        private bool CanSetAsWorkingDay()
        {
            return WorkableDayType != WorkableDayType.WorkingDay;
        }

        private void SetAsWorkingDay()
        {
            WorkableDayType = WorkableDayType.WorkingDay;
        }

        private bool CanSetAsHoliday()
        {
            return WorkableDayType != WorkableDayType.Holiday;
        }

        private void SetAsHoliday()
        {
            WorkableDayType = WorkableDayType.Holiday;
        }

        private void AddTimeEntry()
        {
            Messenger.Default.Send(new EditActivityMessage(Day, _user, _project, _period, true));
        }

        private static bool CanAddTimeEntry()
        {
            return true;
        }

        public void RefreshHours()
        {
            TotalHours = TimeEntries.Sum(t => t.TimeEntryInfo.Hours);
            UnsavedHours =
                TimeEntries.Where(t => t.Origin != TimeEntryOrigin.RedmineService).Sum(t => t.TimeEntryInfo.Hours);
        }

        public bool CanAcceptEntriesFromOtherDay { get; set; }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as RedmineTimeEntry;
            //var targetItem = dropInfo.TargetItem as DayActivity;

            if (sourceItem == null) return; // || targetItem == null || !targetItem.CanAcceptEntriesFromOtherDay) return;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Copy;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as RedmineTimeEntry;
            //var targetItem = dropInfo.TargetItem as DayActivity;

            if (sourceItem == null || TimeEntries.Contains(sourceItem)) return;

            var timeEntryCopy = new TimeEntry
            {
                User = new IdentifiableName {Id = _user.Id},
                Project = new IdentifiableName {Id = _project.ProjectInfo.Id},
                Issue =
                    new IdentifiableName
                    {
                        Id = sourceItem.TimeEntryInfo.Issue.Id,
                        Name = sourceItem.TimeEntryInfo.Issue.Name
                    },
                Activity =
                    new IdentifiableName
                    {
                        Id = sourceItem.TimeEntryInfo.Activity.Id,
                        Name = sourceItem.TimeEntryInfo.Activity.Name
                    },
                SpentOn = Day,
                Hours = sourceItem.TimeEntryInfo.Hours,
                Comments = sourceItem.TimeEntryInfo.Comments,
                CreatedOn = DateTime.Now
            };
            
            var entry = new RedmineTimeEntry(TimeEntryOrigin.NewInLogger, timeEntryCopy);
            Messenger.Default.Send(new AddNewTimeEntryMessage(entry));

        }
    }
}