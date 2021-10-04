namespace LuxAsp.Maps
{
    /// <summary>
    /// Street Address.
    /// </summary>
    public struct MapsAddress
    {
        /// <summary>
        /// Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Region. (Political, Administrative)
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Address Remainder that excludes country and region.
        /// </summary>
        public string Remainder { get; set; }

        /// <summary>
        /// Postal Code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Address Value that represent Full Address.
        /// </summary>
        public string Value { get; set; }
    }
}
