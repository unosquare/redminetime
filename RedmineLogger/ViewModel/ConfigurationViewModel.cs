using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Redmine.Net.Api.Exceptions;
using RedmineLogger.Helpers;
using RedmineLogger.Models;
using RedmineLogger.Models.Messages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace RedmineLogger.ViewModel
{
    public class ConfigurationViewModel: ValidatableViewModelBase
    {
        private readonly ILoggerConfiguration _configuration;
        private string _redmineApiKey;
        private string _redmineUrl;
        private readonly RelayCommand _saveConfigurationCommand;

        public ConfigurationViewModel(ILoggerConfiguration configuration)
        {
            _configuration = configuration;
            _redmineApiKey = configuration.RedmineApiKey;
            _redmineUrl = configuration.RedmineUrl;
            _saveConfigurationCommand = new RelayCommand(SaveConfiguration, CanSaveConfiguration);
        }

        private bool CanSaveConfiguration()
        {
            Uri uriResult;
            var isValidUrl = Uri.TryCreate(RedmineUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return !string.IsNullOrWhiteSpace(RedmineApiKey) && isValidUrl;
        }

        private void SaveConfiguration()
        {
            RedmineService redmineService;
            try
            {
                redmineService = RedmineService.ConnectAndCreate(RedmineUrl, RedmineApiKey);
            }
            catch(RedmineException ex)
            {
                Messenger.Default.Send(new NotificationMessage($"Error connecting to Redmine service: {ex.Message}"));
                redmineService = null;
            }

            if (redmineService == null)
            {
                Messenger.Default.Send(new RedmineConnectionFailMessage());
            }
            else
            {
                // Save configuration
                _configuration.RedmineApiKey = RedmineApiKey;
                _configuration.RedmineUrl = RedmineUrl;

                // Send success message
                Messenger.Default.Send(new RedmineConnectionSuccessMessage(redmineService));
            }
        }

        [Required]
        public string RedmineApiKey
        {
            get { return _redmineApiKey; }
            set
            {
                Set(() => RedmineApiKey, ref _redmineApiKey, value);
                _saveConfigurationCommand.RaiseCanExecuteChanged();
            }
        }

        [Required]
        [RegularExpression(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9]\.[^\s]{2,})", ErrorMessage = @"Must be a valid URL address")]
        public string RedmineUrl
        {
            get { return _redmineUrl; }
            set
            {
                Set(() => RedmineUrl, ref _redmineUrl, value);
                _saveConfigurationCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveConfigurationCommand => _saveConfigurationCommand;
    }
}