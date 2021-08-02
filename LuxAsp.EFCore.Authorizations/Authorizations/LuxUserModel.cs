using LuxAsp.Authorizations;
using LuxAsp.Notations;
using LuxAsp.Utilities;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// User Model.
    /// </summary>
    [Table("LuxUsers")]
    public class LuxUserModel : DataModel
    {
        /// <summary>
        /// Token Id.
        /// </summary>
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Creation Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// Last Write Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Login Name.
        /// </summary>
        [UniqueValue][MaxLength(255)][Required]
        public string LoginName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required][MaxLength(255)]
        public string Password { get; set; }

        /// <summary>
        /// Password Time when user modified latest.
        /// </summary>
        [UniversalDateTime]
        public DateTime PasswordTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Name of the user.
        /// </summary>
        [Required][MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Set Password Value.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public LuxUserModel SetPassword(string Value)
        {
            Password = HashString.Hash("sha256", Value);
            PasswordTime = DateTime.Now;
            return this;
        }

        /// <summary>
        /// Test whether the password is matched or not.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool MatchPassword(string Value)
        {
            if (string.IsNullOrWhiteSpace(Password))
                return false;

            var Algorithm = Password.Split(':', 2, StringSplitOptions.None);
            return Password == HashString.Hash(Algorithm[0], Value);
        }
    }
}
