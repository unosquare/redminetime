using GalaSoft.MvvmLight.Messaging;

namespace Unosquare.RedmineTime.Models.Messages
{
    public class TimeEntrySelectedMessage: MessageBase
    {
        public RedmineTimeEntry TimeEntry { get; set; }

        public TimeEntrySelectedMessage(RedmineTimeEntry timeEntry)
        {
            TimeEntry = timeEntry;
        }
    }
}
