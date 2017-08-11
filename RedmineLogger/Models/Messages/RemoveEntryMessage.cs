using GalaSoft.MvvmLight.Messaging;

namespace RedmineLogger.Models.Messages
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