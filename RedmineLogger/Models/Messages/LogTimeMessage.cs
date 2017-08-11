using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;

namespace RedmineLogger.Models.Messages
{
    public class LogTimeMessage: MessageBase
    {
        public RedmineUserProject UserProject { get; set; }

        public WorkPeriod Period { get; set; }

        public User User { get; set; }

        public LogTimeMessage(User user, RedmineUserProject userProject, WorkPeriod period)
        {
            User = user;
            UserProject = userProject;
            Period = period;
        }
    }
}