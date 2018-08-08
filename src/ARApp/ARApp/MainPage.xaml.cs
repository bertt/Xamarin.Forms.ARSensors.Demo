using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace ARApp
{
	public partial class MainPage : ContentPage
	{
        private IGeolocator locator;
        private Position position;

		public MainPage()
		{
			InitializeComponent();

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var hasPermission = await Utils.CheckPermissions(Permission.Location);
            if (!hasPermission)
                return;

            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

            locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
            Debug.WriteLine("Position Status: {0}", e.Position.Timestamp);
            Debug.WriteLine("Position Latitude: {0}", e.Position.Latitude);
            Debug.WriteLine("Position Longitude: {0}", e.Position.Longitude);
        }
    }
}
