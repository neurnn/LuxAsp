using LuxAsp.Documents;

namespace LuxAsp
{
    public static class LuxDocumentExtensions
    {
        /// <summary>
        /// Add Document Model that uses default Document class and DocumentRepository class.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithDocumentModel(this IDatabaseOptions This)
            => This.WithDocumentModel<LuxDocumentModel, LuxDocumentRepository>();

        /// <summary>
        /// Add Document Model that uses default DocumentRepository class.
        /// </summary>
        /// <typeparam name="TDocumentModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithDocumentModel<TDocumentModel>(this IDatabaseOptions This) where TDocumentModel : LuxDocumentModel, new()
            => This.WithDocumentModel<TDocumentModel, DocumentRepository<TDocumentModel>>();

        /// <summary>
        /// Add Document Model.
        /// </summary>
        /// <typeparam name="TDocumentModel"></typeparam>
        /// <typeparam name="TDocumentRepository"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithDocumentModel<TDocumentModel, TDocumentRepository>(this IDatabaseOptions This)
            where TDocumentModel : LuxDocumentModel, new() where TDocumentRepository : DocumentRepository<TDocumentModel>, new() 
            => This.With<TDocumentModel, TDocumentRepository>();
    }
}