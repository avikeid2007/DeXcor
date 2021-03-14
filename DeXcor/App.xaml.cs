using DeXcor.Services;
using System;
using Windows.ApplicationModel;
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


        public App()
        {
            InitializeComponent();
            // AppCenter.Start("47fe3125-c353-4605-99e8-706dee6980a1", typeof(Analytics), typeof(Crashes));
            UnhandledException += OnAppUnhandledException;
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            this.Suspending += OnSuspending;
            GetApiKey();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ImageDataService.FetchHomeWallPaperListAsync(new Random().Next(1, 5));
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //Telemetry.LogException(e.Exception, "Uncaught");
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }
        private static void GetApiKey()
        {
            string apiKey = "563492ad6f91700001000001768850934594458b9990ea9808e8d8fb";
            if (!string.IsNullOrEmpty(apiKey))
            {
                ImageDataService.ApiKey = apiKey;
            }
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

    }
}
