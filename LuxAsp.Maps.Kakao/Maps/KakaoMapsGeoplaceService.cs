using LuxAsp.Kakao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Maps
{
    public class KakaoMapsGeoplaceService : IMapsGeoplaceService
    {
        private static readonly IEnumerable<MapsGeoplace> EMPTY = new MapsGeoplace[0];
        private HttpClient m_Admin;
        //private HttpClient m_Rest = new HttpClient();

        public KakaoMapsGeoplaceService(
            IServiceProvider Services, MapsApiCredentials Credentials)
        {
            this.Credentials = Credentials;
        }

        /// <summary>
        /// Geopoint Kind.
        /// </summary>
        public MapsGeopointKind GeopointKind => MapsGeopointKind.WGS84;

        /// <summary>
        /// Kakao Maps Api Credentials.
        /// </summary>
        public MapsApiCredentials Credentials { get; }

        /// <summary>
        /// Get Admin Http Api Client.
        /// </summary>
        /// <returns></returns>
        protected HttpClient GetAdminHttpClient()
        {
            lock (this)
            {
                if (m_Admin is null)
                    Credentials.SetAdminApi(m_Admin = new HttpClient());

                return m_Admin;
            }
        }

        private class R
        {
            public class MetaData
            {
                [JsonProperty("total_count")]
                public int Totals { get; set; }

                [JsonProperty("pageable_count")]
                public int Pagables { get; set; }

                [JsonProperty("is_end")]
                public bool IsEndOf { get; set; }
            }

            public class Address
            {
                [JsonProperty("address_name")]
                public string FullAddress { get; set; }

                [JsonProperty("region_1depth_name")]
                public string RegionDepth1 { get; set; }

                [JsonProperty("region_2depth_name")]
                public string RegionDepth2 { get; set; }

                [JsonProperty("region_3depth_name")]
                public string RegionDepth3 { get; set; }

                [JsonProperty("region_3depth_-h_name")]
                public string RegionDepth3_H { get; set; }

                [JsonProperty("h_code")]
                public string PoliticalCode { get; set; }

                [JsonProperty("b_code")]
                public string LegalCode { get; set; }

                /// <summary>
                /// Y/N in string.
                /// </summary>
                [JsonProperty("mountain_yn")]
                public string IsMountain { get; set; }

                [JsonProperty("main_address_no")]
                public string MajorNumber { get; set; }

                [JsonProperty("sub_address_no")]
                public string MinorNumber { get; set; }
            }

            public class RoadAddress
            {
                [JsonProperty("address_name")]
                public string FullAddress { get; set; }

                [JsonProperty("region_1depth_name")]
                public string RegionDepth1 { get; set; }

                [JsonProperty("region_2depth_name")]
                public string RegionDepth2 { get; set; }

                [JsonProperty("region_3depth_name")]
                public string RegionDepth3 { get; set; }

                [JsonProperty("road_name")]
                public string RoadName { get; set; }

                /// <summary>
                /// Y/N in string.
                /// </summary>
                [JsonProperty("underground_yn")]
                public string IsUnderground { get; set; }

                [JsonProperty("main_building_no")]
                public string MajorNumber { get; set; }

                [JsonProperty("sub_building_no")]
                public string MinorNumber { get; set; }

                [JsonProperty("building_name")]
                public string BuildingName { get; set; }

                [JsonProperty("zone_no")]
                public string PostalCode { get; set; }
            }

            public struct Document
            {
                [JsonProperty("address")]
                public string FullAddress { get; set; }

                [JsonProperty("x")]
                public string Longitude { get; set; }

                [JsonProperty("y")]
                public string Latitude { get; set; }

                [JsonProperty("address")]
                public Address Address { get; set; }

                [JsonProperty("road_address")]
                public RoadAddress RoadAddress { get; set; }

            }

            [JsonProperty("meta")]
            public MetaData Meta { get; set; }

            [JsonProperty("documents")]
            public Document[] Documents { get; set; }
        }

        /// <summary>
        /// Query Geoplace asynchronously.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MapsGeoplace>> QueryAsync(string Address)
        {
            IEnumerable<R.Document> Documents = null;
            var Http = GetAdminHttpClient();
            var Page = 1;

            while (true)
            {
                var Path =
                    $"{MapsApiConstants.API_QUERY_ADDRESS}" +
                    $"?query={Uri.EscapeUriString(Address)}" +
                    $"&page={Page}&size=20";

                try
                {
                    var Resp = JsonConvert.DeserializeObject
                        <R>(await Http.GetStringAsync(Path));

                    if (Resp is null || Resp.Meta is null)
                        return EMPTY;

                    if (Documents is null)
                        Documents = Resp.Documents;

                    else Documents = Documents.Concat(Resp.Documents);

                    if (!Resp.Meta.IsEndOf)
                        break;
                }

                catch (Exception) { throw; }
                break;
            }

            return Convert(Documents);
        }

        /// <summary>
        /// Query Geoplace asynchronously.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MapsGeoplace>> QueryAsync(MapsGeopoint Geopoint)
        {
            IEnumerable<R.Document> Documents = null;
            var Http = GetAdminHttpClient();
            var Page = 1;
            var Coord = "WGS84";

            switch(Geopoint.Kind)
            {
                case MapsGeopointKind.Unspec:
                case MapsGeopointKind.WGS84: break;

                case MapsGeopointKind.TM: Coord = "TM"; break;
                case MapsGeopointKind.WTM: Coord = "WTM"; break;
                case MapsGeopointKind.KCNM: Coord = "CONGNAMUL"; break;
                case MapsGeopointKind.WKCNM: Coord = "WCONGNAMUL"; break;

                default: throw new NotSupportedException(
                    $"No geo-coord kind supported: {Geopoint.Kind}.");
            }

            while (true)
            {
                var Path =
                    $"{MapsApiConstants.API_QUERY_ADDRESS}?input_coord={Coord}" +
                    $"&x={Geopoint.Longitude}&y={Geopoint.Latitude}" +
                    $"&page={Page}&size=20";

                try
                {
                    var Resp = JsonConvert.DeserializeObject
                        <R>(await Http.GetStringAsync(Path));

                    if (Resp is null || Resp.Meta is null)
                        return EMPTY;

                    if (Documents is null)
                        Documents = Resp.Documents;

                    else Documents = Documents.Concat(Resp.Documents);

                    if (!Resp.Meta.IsEndOf)
                        break;
                }

                catch (Exception) { throw; }
                break;
            }

            return Convert(Documents);
        }

        /// <summary>
        /// Convert Kakao Response Documents to Geoplace objects.
        /// </summary>
        /// <param name="Documents"></param>
        /// <returns></returns>
        private IEnumerable<MapsGeoplace> Convert(IEnumerable<R.Document> Documents)
        {
            return Documents.Select(X =>
            {
                if (X.RoadAddress is null)
                    return null;

                var Geoplace = new MapsGeoplace();

                if (double.TryParse(X.Latitude, out var Latitude) &&
                    double.TryParse(X.Longitude, out var Longitude))
                {
                    Geoplace.SetGeopoint(new MapsGeopoint(
                        GeopointKind, Latitude, Longitude));
                }

                Geoplace.SetAddress(new MapsAddress
                {
                    Country = "대한민국",
                    Value = X.RoadAddress.FullAddress,
                    Region = string.Join(" ", new string[] {
                        X.RoadAddress.RegionDepth1,
                        X.RoadAddress.RegionDepth2
                    }.Where(X => !string.IsNullOrWhiteSpace(X))),
                    Remainder = string.Join(" ", new string[] {
                        X.RoadAddress.RoadName,
                        string.Join('-', new string []
                        {
                            X.RoadAddress.MajorNumber,
                            X.RoadAddress.MinorNumber
                        }.Where(X => !string.IsNullOrWhiteSpace(X))),
                        X.RoadAddress.BuildingName
                    }.Where(X => !string.IsNullOrWhiteSpace(X))),
                    PostalCode = X.RoadAddress.PostalCode ?? ""
                });

                if (X.Address != null)
                {
                    Geoplace.SetKoreanAddress(new MapsKoreanAddress
                    {
                        Value = X.Address.FullAddress,
                        MajorNumber = X.Address.MajorNumber,
                        MinorNumber = X.Address.MinorNumber,
                        LegalCode = X.Address.LegalCode,
                        PoliticalCode = X.Address.PoliticalCode
                    });
                }

                return Geoplace;
            });
        }
    }
}
