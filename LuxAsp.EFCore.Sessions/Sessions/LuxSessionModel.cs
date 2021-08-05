using LuxAsp.Notations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LuxAsp.Sessions
{
    public class LuxSessionModel : DataModel
    {
        private string m_Base64;
        private byte[] m_Bytes;

        /// <summary>
        /// Token Id.
        /// </summary>
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Creation Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Last Write Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime LastWriteTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Expiration Extends Cycle.
        /// </summary>
        [TimeSpanInSeconds]
        public TimeSpan ExpirationExtends { get; set; } = TimeSpan.FromHours(14);

        /// <summary>
        /// Expiration Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Base64 encoded bytes.
        /// </summary>
        [Required(AllowEmptyStrings = true)][Column(TypeName = "longtext")]
        public string Base64
        {
            get => m_Base64;
            set
            {
                if (m_Base64 != value)
                {
                    m_Base64 = value;
                    m_Bytes = null;
                }
            }
        }

        /// <summary>
        /// Gets or Sets decoded bytes.
        /// </summary>
        [NotMapped]
        public byte[] Bytes
        {
            get
            {
                if (m_Bytes is null)
                {
                    if (string.IsNullOrWhiteSpace(m_Base64))
                         m_Bytes = new byte[0];
                    else m_Bytes = Convert.FromBase64String(m_Base64);
                }

                return m_Bytes;
            }

            set
            {
                if (value is null)
                {
                    m_Base64 = "";
                    m_Bytes = new byte[0];
                    return;
                }

                m_Base64 = Convert.ToBase64String(value);
                m_Bytes = value;
            }
        }
    }
}
