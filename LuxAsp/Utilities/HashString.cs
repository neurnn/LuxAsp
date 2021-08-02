using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Utilities
{
    public sealed class HashString
    {
        /// <summary>
        /// Hash Input using SHA256.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Hash(string Input) => Hash("sha256", Input);

        /// <summary>
        /// Hash Input using specified algorithm.<br />
        /// Supported Algorithms: md5, sha1, sha256, sha384, sha512.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Hash(string Algorithm, string Input)
        {
            var Algo = NewAlgorithm(Algorithm = Algorithm ?? "sha256");
            if (Algo is null)
                throw new ArgumentException($"No such algorithm supported: {Algorithm}.");

            using (Algo)
            {
                var Bytes = Encoding.UTF8.GetBytes(Input);
                return $"{Algo}:{BitConverter.ToString(Bytes).Replace("-", "")}";
            }
        }

        /// <summary>
        /// Create a HashAlgorithm instance by algorithm name.
        /// </summary>
        /// <param name="Algorithm"></param>
        /// <returns></returns>
        private static HashAlgorithm NewAlgorithm(string Algorithm)
        {
            switch ((Algorithm ?? "").ToLower())
            {
                case "md5":
                    return MD5.Create();

                case "sha1":
                    return SHA1.Create();

                case "sha256":
                    return SHA256.Create();

                case "sha384":
                    return SHA384.Create();

                case "sha512":
                    return SHA512.Create();

                default:
                    break;
            }

            return null;
        }
    }
}
