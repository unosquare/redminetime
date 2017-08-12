using Redmine.Net.Api.Types;

namespace Unosquare.RedmineTime.Models.Messages
{
    public class ProjectSelectedMessage: LogTimeMessage
    {
        public ProjectSelectedMessage(User user, RedmineUserProject userProject, WorkPeriod period)
            : base(user, userProject, period)
        {
        }
    }
}