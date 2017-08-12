using GalaSoft.MvvmLight.Messaging;

namespace Unosquare.RedmineTime.Models.Messages
{
    public class RemoveEntryMessage: MessageBase
    {
        public RedmineTimeEntry TimeEntry { get; }

        public RemoveEntryMessage(RedmineTimeEntry timeEntry)
        {
            TimeEntry = timeEntry;
        }
    }
}