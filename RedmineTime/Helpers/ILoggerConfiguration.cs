namespace Unosquare.RedmineTime.Helpers
{
    public interface ILoggerConfiguration
    {
        string RedmineApiKey { get; set; }
        string RedmineUrl { get; set; }

    }
}