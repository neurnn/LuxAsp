using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Kakao
{
    public sealed class MapsApiCredentials
    {
        /// <summary>
        /// REST API Key. (Bearer)
        /// </summary>
        public string RestApiKey { get; set; }

        /// <summary>
        /// Admin API Key. (Kakao AK)
        /// </summary>
        public string AdminApiKey { get; set; }

        /// <summary>
        /// Set Administrator API Endpoint.
        /// </summary>
        /// <param name="Http"></param>
        /// <returns></returns>
        internal HttpClient SetAdminApi(HttpClient Http)
        {
            Http.BaseAddress = new Uri(MapsApiConstants.URI_API_ENDPOINT);
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                MapsApiConstants.HTTP_AdminApiKey_Scheme, AdminApiKey);

            return Http;
        }

        /// <summary>
        /// Set Rest API Endpoint.
        /// </summary>
        /// <param name="Http"></param>
        /// <returns></returns>
        internal HttpClient SetRestApi(HttpClient Http)
        {
            Http.BaseAddress = new Uri(MapsApiConstants.URI_API_ENDPOINT);
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                MapsApiConstants.HTTP_RestApiKey_Scheme, RestApiKey);

            return Http;
        }
    }
}
