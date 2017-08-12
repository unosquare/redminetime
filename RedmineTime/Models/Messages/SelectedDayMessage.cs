using GalaSoft.MvvmLight.Messaging;
using System;

namespace Unosquare.RedmineTime.Models.Messages
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