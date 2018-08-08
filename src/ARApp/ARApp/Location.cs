using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace ARApp
{
    public struct Location
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;

        const double DEGREES_TO_RADIANS = Math.PI / 180.0;
        const double WGS84_A = 6378137.0;
        const double WGS84_E = 8.1819190842622e-2;

        public Location(double latitude, double longitude, double altitude = 0)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public const double EarthRadius = 6378137.0;
        public const double EarthEccentricity = 8.1819190842622e-2;

        // ECEF: earth-centered, earth-fixed
        public Vector3d EcefPosition
        {
            get
            {
                double clat = Math.Cos(Latitude * DEGREES_TO_RADIANS);
                double slat = Math.Sin(Latitude * DEGREES_TO_RADIANS);
                double clon = Math.Cos(Longitude * DEGREES_TO_RADIANS);
                double slon = Math.Sin(Longitude * DEGREES_TO_RADIANS);

                double N = WGS84_A / Math.Sqrt(1.0 - WGS84_E * WGS84_E * slat * slat);


                var x = (N + Altitude) * clat * clon;
                var y = (N + Altitude) * clat * slon;
                var z = (N + (1.0 - WGS84_E * WGS84_E) + Altitude) * slat;

                return new Vector3d(x, y, z);
            }
        }

        public Vector3d EnuPosition(Vector3d cameraPosition)
        {
            var position = EcefPosition;
            double clat = Math.Cos(Latitude * DEGREES_TO_RADIANS);
            double slat = Math.Sin(Latitude * DEGREES_TO_RADIANS);
            double clon = Math.Cos(Longitude * DEGREES_TO_RADIANS);
            double slon = Math.Sin(Longitude * DEGREES_TO_RADIANS);
            double dx = cameraPosition.X - position.X;
            double dy = cameraPosition.Y - position.Y;
            double dz = cameraPosition.Z - position.Z;

            var e = -slon * dx + clon * dy;
            var n = -slat * clon * dx - slat * slon * dy + clat * dz;
            var u = clat * clon * dx + clat * slon * dy + slat * dz;
            return new Vector3d(e, n, u);
        }

        public Location OffsetAltitude(double offset)
        {
            return new Location(Latitude, Longitude, Altitude + offset);
        }

        public override string ToString()
        {
            return string.Format("[Lat={0}, Lon={1}, Alt={2}]", Latitude, Longitude, Altitude);
        }
    }
}
