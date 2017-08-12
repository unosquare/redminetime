using GalaSoft.MvvmLight.Messaging;

namespace Unosquare.RedmineTime.Models.Messages
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