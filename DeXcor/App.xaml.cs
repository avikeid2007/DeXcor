using DeXcor.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Configuration;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace DeXcor
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }
        public static string ApiKey { get; private set; }
        public App()
        {
            InitializeComponent();
            AppCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
            UnhandledException += OnAppUnhandledException;
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            GetApiKey();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }
        private static void GetApiKey()
        {
            string apiKey = ConfigurationManager.AppSettings["apiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKey = apiKey;
            }
        }
        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

    }
}
