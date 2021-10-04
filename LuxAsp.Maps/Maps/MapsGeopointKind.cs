namespace LuxAsp.Maps
{
    public enum MapsGeopointKind
    {
        Unspec,

        TM = 0x080000,
        UTM = 0x040000,
        WTM = 0x080000 | 0x020000,

        /// <summary>
        /// WGS84: Google Maps, Yahoo!, MS Live, etc...
        /// </summary>
        WGS84 = 0x020000,
        Bessel = 0x010000,

        // --------- Country Specs, --> ([Country Code] << 16) + Index.

        /// <summary>
        /// KTM based on TM.
        /// </summary>
        KTM = TM + (82 << 16),

        /// <summary>
        /// WKTM based on WTM.
        /// </summary>
        WKTM = WTM + (82 << 16),

        /// <summary>
        /// Congnamul based on Bessel: Daum, Congnamul.
        /// </summary>
        KCNM = Bessel + (82 << 16) + 1,

        /// <summary>
        /// Congnamul based on WGS84: Daum, Congnamul.
        /// </summary>
        WKCNM = WGS84 + (82 << 16),
    }
}
