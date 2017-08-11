using GalaSoft.MvvmLight.Messaging;

namespace RedmineLogger.Models.Messages
{
    public class AddNewTimeEntryMessage: MessageBase
    {
        public RedmineTimeEntry Entry { get; set; }

        public AddNewTimeEntryMessage(RedmineTimeEntry entry)
        {
            Entry = entry;
        }
    }
}