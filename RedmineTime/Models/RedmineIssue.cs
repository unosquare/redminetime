using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace Unosquare.RedmineTime.Models
{
    public class RedmineIssue
    {
        private readonly RedmineManager _manager;

        public RedmineIssue(RedmineManager manager, Issue issueInfo)
        {
            IssueInfo = issueInfo;
            _manager = manager;
        }

        public Issue IssueInfo { get; set; }

        public override string ToString()
        {
            return IssueInfo != null ? IssueInfo.Subject : base.ToString();
        }
    }
}