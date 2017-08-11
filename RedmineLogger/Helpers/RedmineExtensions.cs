using System;

namespace RedmineLogger.Helpers
{
    public static class RedmineExtensions
    {

        public static string AsRedmineDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        
    }
}