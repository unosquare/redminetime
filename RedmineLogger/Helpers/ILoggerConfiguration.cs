namespace RedmineLogger.Helpers
{
    public interface ILoggerConfiguration
    {
        string RedmineApiKey { get; set; }
        string RedmineUrl { get; set; }

    }
}