using LuxAsp.Notations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxAsp.Documents
{
    /// <summary>
    /// Base class for Document Models.
    /// </summary>
    [Table("LuxDocuments")]
    public class LuxDocumentModel : DataModel
    {
        /// <summary>
        /// Document Id.
        /// </summary>
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Owner Id. When this value set Guid.Empty, it means that the system generated document.
        /// </summary>
        public Guid OwnerId { get; set; } = Guid.Empty;

        /// <summary>
        /// Category. When this value set Guid.Empty, it means that the document has no category.
        /// </summary>
        public Guid CategoryId { get; set; } = Guid.Empty;

        /// <summary>
        /// Publishing State.
        /// </summary>
        public LuxDocumentState State { get; set; } = LuxDocumentState.Writing;

        /// <summary>
        /// Creation Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Last Write Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Last State Change Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime LastStateTime { get; set; }

        /// <summary>
        /// Title of the document.
        /// </summary>
        [Required(AllowEmptyStrings = true)][MaxLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// Content of the document.
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string Content { get; set; }
    }
}
