using LuxAsp.Notations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace LuxAsp.Documents
{
    public class LuxDocumentRepository : DocumentRepository<LuxDocumentModel>
    {

    }

    public class DocumentRepository<TDocumentModel> 
        : Repository<TDocumentModel> where TDocumentModel : LuxDocumentModel, new()
    {
        /// <summary>
        /// Create a new Document asynchronously.
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="State"></param>
        /// <param name="Content"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public async Task<TDocumentModel> CreateAsync(string Title, 
            LuxDocumentState State, string Content, Action<TDocumentModel> Callback = null)
        {
            return await CreateAsync(() =>
            {
                var New = new TDocumentModel();

                New.Title = Title;
                New.State = State;
                New.Content = Content;

                Callback?.Invoke(New);
                return New;
            });
        }

        /// <summary>
        /// Sets the CreationTime and LastWriteTime property before saving changes.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected override async Task OnSaveRequest(TDocumentModel Entity, Func<Task<bool>> Next)
        {
            if (Entity.IsNew)
                Entity.CreationTime = DateTime.Now;

            if (Entity.IsNew || Entity.IsChanged)
            {
                Entity.LastWriteTime = DateTime.Now;
                Entity.LastStateTime = DateTime.Now;
            }

            await base.OnSaveRequest(Entity, Next);
        }
    }
}
