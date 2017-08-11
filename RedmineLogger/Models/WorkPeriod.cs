using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

namespace RedmineLogger.Models
{
    public class WorkPeriod: ViewModelBase
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public WorkPeriod()
        {
            
        }

        public IEnumerable<DateTime> EachDay
        {
            get
            {
                for (var day = StartDate.Date; day.Date <= EndDate.Date; day = day.AddDays(1))
                    yield return day;
            }
        }

        public WorkPeriod(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { Set(() => StartDate, ref _startDate, value); }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set { Set(() => EndDate, ref _endDate, value); }
        }

        public static WorkPeriod Current
        {
            get
            {
                var today = DateTime.Today;
                var startDay = today.Day > 15 ? 16 : 1;
                var endDay = today.Day > 15 ? DateTime.DaysInMonth(today.Year, today.Month) : 15;
                return new WorkPeriod(new DateTime(today.Year, today.Month, startDay),
                    new DateTime(today.Year, today.Month, endDay));
            }
        }

    }
}