using Redmine.Net.Api;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using RedmineLogger.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace RedmineLogger.Models
{
    public class RedmineUserProject
    {
        private readonly RedmineManager _manager;
        private readonly User _user;
        private IEnumerable<TimeEntryActivity> _activities;
        private IEnumerable<RedmineIssue> _issues;

        public RedmineUserProject(RedmineManager manager, Project projectInfo, User user)
        {
            ProjectInfo = projectInfo;
            _manager = manager;
            _user = user;
        }

        public Project ProjectInfo { get; set; }

        public IEnumerable<RedmineIssue> Issues => _issues ?? (_issues = GetIssuesFromServer());

        public IEnumerable<TimeEntryActivity> Activities
            => _activities ?? (_activities = _manager.GetObjects<TimeEntryActivity>());

        public override string ToString()
        {
            return ProjectInfo != null ? ProjectInfo.Name : base.ToString();
        }

        public IEnumerable<TimeEntry> GetTimeEntries(WorkPeriod period)
        {
            var parameters = new NameValueCollection
            {
                {RedmineKeys.SPENT_ON, $"><{period.StartDate.AsRedmineDate()}|{period.EndDate.AsRedmineDate()}"},
                {RedmineKeys.USER_ID, $"{_user.Id}"},
                {RedmineKeys.INCLUDE, "issues"}
            };

            var timeEntries = _manager.GetObjects<TimeEntry>(parameters);
            foreach (var timeEntry in timeEntries)
            {
                timeEntry.Issue.Name =
                    Issues.FirstOrDefault(issue => issue.IssueInfo.Id == timeEntry.Issue.Id)?.IssueInfo?.Subject;
            }
            return timeEntries;
        }

        public async Task<TimeEntry> LogTime(Issue issue, decimal hours, TimeEntryActivity activity, DateTime day, string comments)
        {
            var timeEntry = new TimeEntry
            {
                Project = issue.Project,
                Issue = new IdentifiableName {Id = issue.Id},
                Activity = new IdentifiableName {Id = activity.Id},
                Comments = comments,
                SpentOn = day.Date,
                User = new IdentifiableName {Id = _user.Id},
                Hours = hours
            };
            var savedEntry = await LogTime(timeEntry);
            return savedEntry;
        }

        public async Task<TimeEntry> LogTime(RedmineTimeEntry entry)
        {
            if (entry.Origin == TimeEntryOrigin.RedmineService)
            {
                throw new InvalidOperationException("Cannot log time entries originated from Redmine service");
            }
            var savedEntry = await LogTime(entry.TimeEntryInfo);
            return savedEntry;
        }

        private async Task<TimeEntry> LogTime(TimeEntry entry)
        {
            // Avoid problems with issue names lookup if numerical Id is available
            if(entry.Issue.Id != 0)
                entry.Issue.Name = null;
            if (entry.Activity.Id != 0)
                entry.Activity.Name = null;

            var savedEntry = await _manager.CreateObjectAsync(entry);
            return savedEntry;
        }


        private IEnumerable<RedmineIssue> GetIssuesFromServer()
        {
            var parameters = new NameValueCollection
            {
                {RedmineKeys.ASSIGNED_TO_ID, $"{_user.Id}"}
            };
            return _manager.GetObjects<Issue>(parameters).Select(i => new RedmineIssue(_manager, i));
        }

    }
}