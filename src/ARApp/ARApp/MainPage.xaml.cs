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
        private PointOfInterest poi;

		public MainPage()
		{
			InitializeComponent();
            poi = new PointOfInterest
            {
                Name = "Domtoren",
                Latitude = 52.090855,
                Longitude = 5.121274
            };

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
            Debug.WriteLine("Position Altitude: {0}", e.Position.Altitude);
            var cameraLocation = new Location(e.Position.Latitude, e.Position.Longitude, e.Position.Altitude);
            var cameraEcef = cameraLocation.EcefPosition;

            var poiLocation = new Location(poi.Latitude, poi.Longitude);
            var poiEcef = poiLocation.EcefPosition;


        }
    }
}
