using LuxAsp.Notations;

namespace LuxAsp.Documents
{
    /// <summary>
    /// Document State enum.
    /// </summary>
    public enum LuxDocumentState
    {
        /// <summary>
        /// The document is not submitted yet.
        /// </summary>
        Writing = 0,

        /// <summary>
        /// The document has submitted but, not published.
        /// </summary>
        Submitted,

        /// <summary>
        /// The document has published.
        /// </summary>
        Published,

        /// <summary>
        /// The document has blinded.
        /// </summary>
        Blinded
    }
}
