using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;
using RedmineLogger.Models;
using RedmineLogger.Models.Messages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;

namespace RedmineLogger.ViewModel
{
    public class ActivityDetailsViewModel : ValidatableViewModelBase
    {
        private readonly RelayCommand _removeActivityCommand;
        private readonly RelayCommand _saveActivityCommand;
        private ObservableCollection<TimeEntryActivity> _activities;
        private string _comments;
        private decimal _hours;
        private ObservableCollection<RedmineIssue> _issues;
        private TimeEntryDetailsMode _mode;
        private WorkPeriod _period;
        private RedmineUserProject _project;
        private TimeEntryActivity _selectedActivity;
        private DateTime _selectedDate;
        private RedmineIssue _selectedIssue;
        private User _user;

        public ActivityDetailsViewModel()
        {
            Messenger.Default.Register<TimeEntrySelectedMessage>(this, InitializeSelected);
            Messenger.Default.Register<LogTimeMessage>(this, Initialize);
            Messenger.Default.Register<SelectedDayMessage>(this, message => SetDay(message.Day));
            Messenger.Default.Register<EditActivityMessage>(this, Edit);
            _saveActivityCommand = new RelayCommand(SaveActivity, CanSaveActivity);
            _removeActivityCommand = new RelayCommand(RemoveActivity, CanRemoveActivity);

            if (IsInDesignMode)
            {
            }
        }

        public TimeEntryDetailsMode Mode
        {
            get { return _mode; }
            set
            {
                Set(() => Mode, ref _mode, value);
                _saveActivityCommand.RaiseCanExecuteChanged();
            }
        }


        public ObservableCollection<RedmineIssue> Issues
        {
            get { return _issues ?? (_issues = new ObservableCollection<RedmineIssue>()); }
            set { Set(() => Issues, ref _issues, value); }
        }

        [Required]
        public RedmineIssue SelectedIssue
        {
            get { return _selectedIssue; }
            set
            {
                Set(() => SelectedIssue, ref _selectedIssue, value);
                _saveActivityCommand.RaiseCanExecuteChanged();
            }
        }

        [Required]
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { Set(() => SelectedDate, ref _selectedDate, value); }
        }

        [Required]
        [Range(0.5,24)]
        public decimal Hours
        {
            get { return _hours; }
            set { Set(() => Hours, ref _hours, value); }
        }
        
        public string Comments
        {
            get { return _comments; }
            set { Set(() => Comments, ref _comments, value); }
        }

        public ObservableCollection<TimeEntryActivity> Activities
        {
            get { return _activities ?? (_activities = new ObservableCollection<TimeEntryActivity>()); }
            set { Set(() => Activities, ref _activities, value); }
        }

        [Required]
        public TimeEntryActivity SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                Set(() => SelectedActivity, ref _selectedActivity, value);
                _saveActivityCommand.RaiseCanExecuteChanged();
            }
        }

        public WorkPeriod Period
        {
            get { return _period; }
            set { Set(() => Period, ref _period, value); }
        }


        public ICommand RemoveActivityCommand => _removeActivityCommand;

        public ICommand SaveActivityCommand => _saveActivityCommand;

        private void Edit(EditActivityMessage arguments)
        {
            SetDay(arguments.Day);
            _user = arguments.User;
            _project = arguments.UserProject;
            if (arguments.IsNew)
            {
            }
        }

        private void InitializeSelected(TimeEntrySelectedMessage arguments)
        {
        }

        private void SetDay(DateTime day)
        {
            SelectedDate = day.Date;
        }

        private void Initialize(LogTimeMessage arguments)
        {
            Issues.Clear();
            var issuesFromProject = arguments.UserProject.Issues;
            foreach (var issue in issuesFromProject)
            {
                Issues.Add(issue);
            }
            SelectedIssue = Issues.FirstOrDefault();
            var activities = arguments.UserProject.Activities;
            Activities.Clear();
            foreach (var activity in activities)
            {
                Activities.Add(activity);
            }
            SelectedDate = arguments.Period.StartDate;
            Period = arguments.Period;
        }

        private void RemoveActivity()
        {
        }

        private bool CanRemoveActivity()
        {
            return false;
        }

        private void SaveActivity()
        {
            var entry = new RedmineTimeEntry(TimeEntryOrigin.NewInLogger, CreateTimeEntry());
            Messenger.Default.Send(new AddNewTimeEntryMessage(entry));
            //Mode = TimeEntryDetailsMode.Display;
        }

        private TimeEntry CreateTimeEntry()
        {
            var entry = new TimeEntry
            {
                User = new IdentifiableName {Id = _user.Id},
                Project = new IdentifiableName {Id = _project.ProjectInfo.Id},
                Issue = new IdentifiableName {Id = SelectedIssue.IssueInfo.Id, Name = SelectedIssue.IssueInfo.Subject},
                Activity = new IdentifiableName {Id = SelectedActivity.Id, Name = SelectedActivity.Name},
                Comments = Comments,
                CreatedOn = DateTime.Now,
                Hours = Hours,
                SpentOn = SelectedDate.Date
            };
            return entry;
        }

        private bool CanSaveActivity()
        {
            return SelectedActivity != null && SelectedIssue != null && Hours > 0 &&
                   (Mode == TimeEntryDetailsMode.AddNew || Mode == TimeEntryDetailsMode.Edit);
        }

    }
}