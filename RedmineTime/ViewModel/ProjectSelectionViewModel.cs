using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Unosquare.RedmineTime.Models;
using Unosquare.RedmineTime.Models.Messages;

namespace Unosquare.RedmineTime.ViewModel
{
    public class ProjectSelectionViewModel : ViewModelBase
    {
        private readonly RelayCommand _logTimeCommand;

        private readonly RedmineService _service;
        private WorkPeriod _period;
        private ObservableCollection<RedmineUserProject> _projects;
        private RedmineUserProject _selectedUserProject;
        private User _user;

        public ProjectSelectionViewModel(RedmineService service)
        {
            _service = service;
            _logTimeCommand = new RelayCommand(LogTime, CanLogTime);
            if (IsInDesignMode)
            {
                User = new User
                {
                    FirstName = "Tester",
                    LastName = "Probe",
                    Email = "tester.probe@unsoquare.com"
                };
                Period = WorkPeriod.Current;
            }
            else
            {
                InitializeData();
            }
        }

        public User User
        {
            get { return _user; }
            set { Set(() => User, ref _user, value); }
        }

        public ObservableCollection<RedmineUserProject> Projects
        {
            get { return _projects; }
            set { Set(() => Projects, ref _projects, value); }
        }

        public RedmineUserProject SelectedUserProject
        {
            get { return _selectedUserProject; }
            set
            {
                Set(() => SelectedUserProject, ref _selectedUserProject, value);
                _logTimeCommand.RaiseCanExecuteChanged();
            }
        }

        public WorkPeriod Period
        {
            get { return _period; }
            set { Set(() => Period, ref _period, value); }
        }

        public ICommand LogTimeCommand => _logTimeCommand;

        private bool CanLogTime()
        {
            return SelectedUserProject != null;
        }

        private void LogTime()
        {
            Messenger.Default.Send(new ProjectSelectedMessage(User, SelectedUserProject, Period));
        }

        private void InitializeData()
        {
            User = _service.User;
            Projects = new ObservableCollection<RedmineUserProject>(_service.Projects);
            Period = WorkPeriod.Current;
        }
    }
}