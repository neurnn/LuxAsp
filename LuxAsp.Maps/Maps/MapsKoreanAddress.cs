namespace LuxAsp.Maps
{
    /// <summary>
    /// Korean Ji-beon Address.
    /// </summary>
    public struct MapsKoreanAddress
    {
        /// <summary>
        /// Address Value that represent Full Address.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Political Code. (Hang-jeong)
        /// </summary>
        public string PoliticalCode { get; set; }

        /// <summary>
        /// Legal Code. (Bub-jeong)
        /// </summary>
        public string LegalCode { get; set; }

        /// <summary>
        /// Major number of the Ji-beon value.
        /// </summary>
        public string MajorNumber { get; set; }

        /// <summary>
        /// Minor number of the Ji-beon value.
        /// </summary>
        public string MinorNumber { get; set; }
    }
}
