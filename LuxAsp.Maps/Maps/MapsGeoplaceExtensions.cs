using System.Collections.Generic;

namespace LuxAsp.Maps
{
    /// <summary>
    /// Geoplace Extensions.
    /// </summary>
    public static class MapsGeoplaceExtensions
    {
        /// <summary>
        /// Get Meta Data by its Type.
        /// </summary>
        /// <typeparam name="TMeta"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TMeta GetMetaData<TMeta>(this MapsGeoplace This)
        {
            if (!TryGetMetaData<TMeta>(This, out var _Meta))
                throw new KeyNotFoundException($"No metadata set for: {typeof(TMeta).Name}");

            return _Meta;
        }

        /// <summary>
        /// Try Get Meta by its Type.
        /// </summary>
        /// <typeparam name="TMeta"></typeparam>
        /// <param name="This"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool TryGetMetaData<TMeta>(this MapsGeoplace This, out TMeta Value)
        {
            if (This.MetaData.TryGetValue(typeof(TMeta), out var _Value))
            {
                if (_Value is TMeta _Meta)
                {
                    Value = _Meta;
                    return true;
                }
            }

            Value = default;
            return false;
        }

        /// <summary>
        /// Set Meta Data by its Type.
        /// </summary>
        /// <typeparam name="TMeta"></typeparam>
        /// <param name="This"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static MapsGeoplace SetMetaData<TMeta>(this MapsGeoplace This, TMeta Value)
        {
            This.MetaData[typeof(TMeta)] = Value;
            return This;
        }

        /// <summary>
        /// Get Geopoint of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static MapsGeopoint GetGeopoint(this MapsGeoplace This)
        {
            if (This.TryGetMetaData<MapsGeopoint>(out var Geopoint))
                return Geopoint;

            throw new KeyNotFoundException("No geopoint set.");
        }

        /// <summary>
        /// Try Get Geopoint of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static bool TryGetGeopoint(this MapsGeoplace This, out MapsGeopoint Geopoint)
            => This.TryGetMetaData(out Geopoint);

        /// <summary>
        /// Get Geopoint Kind of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static MapsGeopointKind GetGeopointKind(this MapsGeoplace This)
        {
            if (This.TryGetMetaData<MapsGeopoint>(out var Geopoint))
                return Geopoint.Kind;

            return MapsGeopointKind.Unspec;
        }

        /// <summary>
        /// Set Geopoint of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static MapsGeoplace SetGeopoint(this MapsGeoplace This, MapsGeopoint Geopoint)
            => This.SetMetaData(Geopoint);

        /// <summary>
        /// Get Address information from the place.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static MapsAddress GetAddress(this MapsGeoplace This)
        {
            if (This.TryGetMetaData<MapsAddress>(out var Address))
                return Address;

            throw new KeyNotFoundException("No address information set.");
        }

        /// <summary>
        /// Try Get Address information of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static bool TryGetAddress(this MapsGeoplace This, out MapsAddress Geopoint)
            => This.TryGetMetaData(out Geopoint);

        /// <summary>
        /// Set Address of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static MapsGeoplace SetAddress(this MapsGeoplace This, MapsAddress Geopoint)
            => This.SetMetaData(Geopoint);

        /// <summary>
        /// Get Korean Old Address information from the place.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static MapsKoreanAddress GetKoreanAddress(this MapsGeoplace This)
        {
            if (This.TryGetMetaData<MapsKoreanAddress>(out var Address))
                return Address;

            throw new KeyNotFoundException("No address information set.");
        }

        /// <summary>
        /// Try Get Korean Old Address information of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static bool TryGetKoreanAddress(this MapsGeoplace This, out MapsKoreanAddress Geopoint)
            => This.TryGetMetaData(out Geopoint);

        /// <summary>
        /// Set Korean Old Address of the place.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Geopoint"></param>
        /// <returns></returns>
        public static MapsGeoplace SetKoreanAddress(this MapsGeoplace This, MapsKoreanAddress Geopoint)
            => This.SetMetaData(Geopoint);

    }
}
