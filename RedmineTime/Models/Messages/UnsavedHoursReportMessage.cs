using GalaSoft.MvvmLight.Messaging;

namespace Unosquare.RedmineTime.Models.Messages
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