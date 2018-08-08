using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.Numerics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ARApp
{
	public partial class MainPage : ContentPage
	{
        private IGeolocator locator;
        private Position position;
        private Quaternion orientation;

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


            if (OrientationSensor.IsMonitoring)
                OrientationSensor.Stop();
            else
                OrientationSensor.Start(SensorSpeed.Normal);

            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged; ;

        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            orientation = e.Reading.Orientation;
            Debug.WriteLine($"Reading: X: {orientation.X}, Y: {orientation.Y}, Z: {orientation.Z}, W: {orientation.W}");
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
