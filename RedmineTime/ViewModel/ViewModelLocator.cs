using GalaSoft.MvvmLight.Messaging;
using Unosquare.RedmineTime.Helpers;
using Unosquare.RedmineTime.Models;
using Unosquare.RedmineTime.Models.Messages;

namespace Unosquare.RedmineTime.ViewModel
{
    public class ViewModelLocator
    {
        private RedmineService _redmineService;
        private MainViewModel _mainViewModel;

        public ViewModelLocator()
        {
            Messenger.Default.Register<RedmineConnectionSuccessMessage>(this, SetRedmineService);
            Messenger.Default.Register<ProjectSelectedMessage>(this, ProjectSelected);
        }

        private void ProjectSelected(ProjectSelectedMessage message)
        {
            LogPeriod?.Cleanup();
            LogPeriod = new LogPeriodViewModel();
            ActivityDetails = new ActivityDetailsViewModel();
            Messenger.Default.Send(new LogTimeMessage(message.User, message.UserProject, message.Period));
        }

        private void SetRedmineService(RedmineConnectionSuccessMessage message)
        {
            RedmineService = message.ServiceInstance;
            Messenger.Default.Send(new SelectProjectMessage());
        }

        public RedmineService RedmineService
        {
            get { return _redmineService; }
            set
            {
                _redmineService = value;
                ProjectSelection = new ProjectSelectionViewModel(RedmineService);
            }
        }

        public MainViewModel Main => _mainViewModel ?? (_mainViewModel = new MainViewModel());

        public ActivityDetailsViewModel ActivityDetails { get; private set; } 

        public ProjectSelectionViewModel ProjectSelection { get; set; }

        public LogPeriodViewModel LogPeriod { get; private set; }

        public ConfigurationViewModel Configuration => new ConfigurationViewModel(new LoggerConfiguration());
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}