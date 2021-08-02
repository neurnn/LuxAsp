namespace LuxAsp.Authorizations
{
    public struct LuxAuthenticationGeneratedToken
    {
        public static readonly LuxAuthenticationGeneratedToken Empty = new LuxAuthenticationGeneratedToken();

        /// <summary>
        /// Authorization Token
        /// </summary>
        public LuxAuthenticationToken Token { get; set; }

        /// <summary>
        /// Refresh Token. Refresh request should specify this token 
        /// by `X-Authorization-Refresh` header before the token really expired.
        /// </summary>
        public LuxAuthenticationToken RefreshToken { get; set; }

        /// <summary>
        /// Test whether the generated token is valid or not.
        /// </summary>
        public bool IsValid => Token.IsValid;
    }
}
