using GalaSoft.MvvmLight.Messaging;
using System;

namespace RedmineLogger.Models.Messages
{
    public class SelectedDayMessage: MessageBase
    {
        public DateTime Day { get; set; }

        public SelectedDayMessage(DateTime day)
        {
            Day = day;
        }
    }
}