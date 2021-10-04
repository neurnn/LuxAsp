using System;
using System.Linq;

namespace LuxAsp.Maps
{
    /// <summary>
    /// Maps Geopoint.
    /// Warn: These are different per Maps API provider.
    /// For example, some providers uses TM, but others not.
    /// </summary>
    public struct MapsGeopoint
    {
        public MapsGeopoint(MapsGeopoint Other) : this(Other.Kind, Other.Latitude, Other.Longitude) { }
        public MapsGeopoint(double Latitude, double Longitude) : this(MapsGeopointKind.Unspec, Latitude, Longitude) { }
        public MapsGeopoint(MapsGeopointKind Kind, double Latitude, double Longitude)
        {
            this.Kind = Kind;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        /// <summary>
        /// Gets Maximum Value between Geopoints.
        /// </summary>
        /// <param name="Geopoints"></param>
        /// <returns></returns>
        public static MapsGeopoint Max(params MapsGeopoint[] Geopoints)
        {
            var Point = Geopoints.First();

            foreach(var Each in Geopoints.Skip(1))
            {
                if (Point.Kind != Each.Kind &&
                    Point.Kind != MapsGeopointKind.Unspec &&
                    Each.Kind != MapsGeopointKind.Unspec)
                {
                    throw new ArgumentException(
                        $"Geopoint kind mismatch: {Point.Kind} vs {Each.Kind}");
                }

                var Lat = Math.Max(Each.Latitude, Point.Latitude);
                var Lng = Math.Max(Each.Longitude, Point.Longitude);
                Point = new MapsGeopoint(Point.Kind, Lat, Lng);
            }

            return Point;
        }

        /// <summary>
        /// Gets Minimum Value between Geopoints.
        /// </summary>
        /// <param name="Geopoints"></param>
        /// <returns></returns>
        public static MapsGeopoint Min(params MapsGeopoint[] Geopoints)
        {
            var Point = Geopoints.First();

            foreach (var Each in Geopoints.Skip(1))
            {
                if (Point.Kind != Each.Kind &&
                    Point.Kind != MapsGeopointKind.Unspec &&
                    Each.Kind != MapsGeopointKind.Unspec)
                {
                    throw new ArgumentException(
                        $"Geopoint kind mismatch: {Point.Kind} vs {Each.Kind}");
                }

                var Lat = Math.Min(Each.Latitude, Point.Latitude);
                var Lng = Math.Min(Each.Longitude, Point.Longitude);
                Point = new MapsGeopoint(Point.Kind, Lat, Lng);
            }

            return Point;
        }

        /// <summary>
        /// Kind of the Geopoint.
        /// </summary>
        public MapsGeopointKind Kind { get; }

        /// <summary>
        /// Latitude.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Longitude.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Sum two geopoints to single geopoint.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static MapsGeopoint operator +(MapsGeopoint A, MapsGeopoint B)
        {
            if (A.Kind != B.Kind && 
                A.Kind != MapsGeopointKind.Unspec && 
                B.Kind != MapsGeopointKind.Unspec)
            {
                throw new ArgumentException(
                    $"Geopoint kind mismatch: {A.Kind} vs {B.Kind}");
            }

            return new MapsGeopoint(A.Kind, A.Latitude + B.Latitude, A.Longitude + B.Longitude);
        }

        /// <summary>
        /// Subtract two geopoints to single geopoint.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static MapsGeopoint operator -(MapsGeopoint A, MapsGeopoint B)
        {
            if (A.Kind != B.Kind &&
                A.Kind != MapsGeopointKind.Unspec &&
                B.Kind != MapsGeopointKind.Unspec)
            {
                throw new ArgumentException(
                    $"Geopoint kind mismatch: {A.Kind} vs {B.Kind}");
            }

            return new MapsGeopoint(A.Kind, A.Latitude - B.Latitude, A.Longitude - B.Longitude);
        }

        /// <summary>
        /// Multiply the scalar value to the geopoint.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Scalar"></param>
        /// <returns></returns>
        public static MapsGeopoint operator *(MapsGeopoint Geopoint, double Scalar)
            => new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude * Scalar, Geopoint.Longitude * Scalar);

        /// <summary>
        /// Divide the scalar value to the geopoint.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Scalar"></param>
        /// <returns></returns>
        public static MapsGeopoint operator /(MapsGeopoint Geopoint, double Scalar)
            => new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude / Scalar, Geopoint.Longitude / Scalar);
    }

    /// <summary>
    /// Maps Geopoint Rectangle.
    /// </summary>
    public struct MapsGeopointRectangle
    {
        /// <summary>
        /// Create a new rectangle using single geopoint and size of the stride.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Stride"></param>
        public MapsGeopointRectangle(MapsGeopoint Geopoint, double Stride)
            : this(new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude - Stride * 0.5, Geopoint.Longitude - Stride * 0.5),
                   new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude + Stride * 0.5, Geopoint.Longitude + Stride * 0.5)) { }

        /// <summary>
        /// Create a new rectangle using single geopoint and size of each stride.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Stride"></param>
        public MapsGeopointRectangle(MapsGeopoint Geopoint, double LatitudeStride, double LongitudeStride)
            : this(new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude - LatitudeStride * 0.5, Geopoint.Longitude - LongitudeStride * 0.5),
                   new MapsGeopoint(Geopoint.Kind, Geopoint.Latitude + LatitudeStride * 0.5, Geopoint.Longitude + LongitudeStride * 0.5)) { }

        /// <summary>
        /// Create a new rectangle using multiple geopoints.
        /// </summary>
        /// <param name="Geopoints"></param>
        public MapsGeopointRectangle(params MapsGeopoint[] Geopoints)
        {
            Kind = Geopoints.First().Kind;
            Min = MapsGeopoint.Min(Geopoints);
            Max = MapsGeopoint.Max(Geopoints);
        }

        /// <summary>
        /// Multiply the scalar value to the georect.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Scalar"></param>
        /// <returns></returns>
        public static MapsGeopointRectangle operator *(MapsGeopointRectangle Georect, double Scalar)
            => new MapsGeopointRectangle(Georect.Center, Georect.LatitudeStride * Scalar, Georect.LongitudeStride * Scalar);

        /// <summary>
        /// Divide the scalar value to the georect.
        /// </summary>
        /// <param name="Geopoint"></param>
        /// <param name="Scalar"></param>
        /// <returns></returns>
        public static MapsGeopointRectangle operator /(MapsGeopointRectangle Georect, double Scalar)
            => new MapsGeopointRectangle(Georect.Center, Georect.LatitudeStride / Scalar, Georect.LongitudeStride / Scalar);

        /// <summary>
        /// Kind of the Geopoint.
        /// </summary>
        public MapsGeopointKind Kind { get; }

        /// <summary>
        /// Minimum Value of rectangle.
        /// </summary>
        public MapsGeopoint Min { get; }

        /// <summary>
        /// Maximum Value of rectangle.
        /// </summary>
        public MapsGeopoint Max { get; }

        /// <summary>
        /// Center of the rectangle.
        /// </summary>
        public MapsGeopoint Center => (Min + Max) * 0.5;

        /// <summary>
        /// Latitude Stride.
        /// </summary>
        public double LatitudeStride => (Max - Min).Latitude;

        /// <summary>
        /// Longitude Stride.
        /// </summary>
        public double LongitudeStride => (Max - Min).Longitude;
    }
}
