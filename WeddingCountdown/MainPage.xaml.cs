using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using WeddingCountdown.ViewModel;
using System;

namespace WeddingCountdown
{
    public sealed partial class MainPage
    {
        public MainViewModel Vm => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            Loaded += (s, e) =>
            {
                Vm.RunClock();
            };
        }

        private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Vm.StopClock();
            base.OnNavigatingFrom(e);
        }

        private void weddingDate_DateChanged(Windows.UI.Xaml.Controls.CalendarDatePicker sender, Windows.UI.Xaml.Controls.CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate < DateTimeOffset.Now)
                return;

            Vm.WeddingDate = args.NewDate;
        }
    }
}
