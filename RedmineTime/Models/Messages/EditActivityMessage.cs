using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Types;
using System;

namespace Unosquare.RedmineTime.Models.Messages
{
    public class EditActivityMessage : MessageBase
    {
        public DateTime Day { get; set; }
        public User User { get; set; }
        public RedmineUserProject UserProject { get; set; }
        public WorkPeriod Period { get; set; }

        public bool IsNew { get; set; }

        public EditActivityMessage(DateTime day, User user, RedmineUserProject userProject, WorkPeriod period, bool isNew)
        {
            Day = day;
            User = user;
            UserProject = userProject;
            Period = period;
            IsNew = isNew;
        }
    }
}