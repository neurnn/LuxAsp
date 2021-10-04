namespace LuxAsp.Kakao
{
    internal sealed class MapsApiConstants
    {
        public const string HTTP_AdminApiKey_Scheme = "KakaoAK";
        public const string HTTP_RestApiKey_Scheme = "bearer";

        public const string URI_API_ENDPOINT = "https://dapi.kakao.com";

        public const string API_QUERY_ADDRESS = "/v2/local/search/address.json";
        public const string API_SEARCH_KEYWORD = "/v2/local/search/keyword.json";
    }
}
