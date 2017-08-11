using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using RedmineLogger.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace RedmineLogger.Models
{
    public class RedmineService
    {
        private User _userInfo;
        private IEnumerable<RedmineUserProject> _projects;

        public static RedmineService ConnectAndCreate(string url, string apiKey)
        {
            var manager = new RedmineManager(url, apiKey);
            manager.GetCurrentUser();
            return new RedmineService(manager);
        }

        public static RedmineService ConnectAndCreate(ILoggerConfiguration configuration)
        {
            return ConnectAndCreate(configuration.RedmineUrl, configuration.RedmineApiKey);
        }

        public RedmineService(RedmineManager manager)
        {
            Manager = manager;
        }

        protected RedmineManager Manager { get; }

        public User User => _userInfo ?? (_userInfo = Manager.GetCurrentUser());

        public IEnumerable<RedmineUserProject> Projects
            =>
                _projects ??
                (_projects = Manager.GetObjects<Project>().Select(p => new RedmineUserProject(Manager, p, User)));

    }
}