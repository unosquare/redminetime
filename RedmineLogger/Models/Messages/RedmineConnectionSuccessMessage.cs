using GalaSoft.MvvmLight.Messaging;

namespace RedmineLogger.Models.Messages
{
    public class RedmineConnectionSuccessMessage: MessageBase
    {

        public RedmineService ServiceInstance { get; private set; }

        public RedmineConnectionSuccessMessage(RedmineService serviceInstance)
        {
            ServiceInstance = serviceInstance;
        }
        
    }
}