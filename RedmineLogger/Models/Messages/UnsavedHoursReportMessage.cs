using GalaSoft.MvvmLight.Messaging;

namespace RedmineLogger.Models.Messages
{
    public class UnsavedHoursReportMessage: MessageBase
    {
        public UnsavedHoursReportMessage(decimal unsavedHours)
        {
            UnsavedHours = unsavedHours;
        }

        public decimal UnsavedHours { get; }
        
    }
}