using LuxAsp.Kakao;
using LuxAsp.Maps;
using System;

namespace LuxAsp
{
    public static class KakaoMapsServiceExtensions
    {
        /// <summary>
        /// Use Kakao Maps for Maps Services.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Credentials"></param>
        /// <returns></returns>
        public static MapsServices UseKakaoMaps(this MapsServices This, MapsApiCredentials Credentials)
        {
            This.Geoplace = Services => new KakaoMapsGeoplaceService(Services, Credentials);
            return This;
        }
    }
}
