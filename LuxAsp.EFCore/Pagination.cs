using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp
{
    /// <summary>
    /// Data Pagination.
    /// </summary>
    public sealed class Pagination<TEntity> : IEnumerable<TEntity> where TEntity : DataModel
    {
        private IQueryable<TEntity> m_Query;
        private IEnumerable<TEntity> m_CurrentView;
        private Repository<TEntity> m_Repository;

        private int m_TotalRecords;
        private int m_PageIndex = 1;

        internal Pagination(Repository<TEntity> Repository, IQueryable<TEntity> Query)
        {
            m_Query = Query;
            m_TotalRecords = m_Query.Count();
            m_CurrentView = m_Query;
            m_Repository = Repository;
        }

        /// <summary>
        /// Set Page No and Items Per Page.
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="ItemsPerPage"></param>
        /// <returns></returns>
        public Pagination<TEntity> SetPage(int PageNo, int ItemsPerPage = 20)
        {
            int Index = Math.Max(PageNo, 1) - 1;
            int Count = Math.Max(ItemsPerPage, 1);

            if (m_PageIndex != Index ||
                this.ItemsPerPage != Count)
            {
                m_PageIndex = Index;
                this.ItemsPerPage = Count;
                m_CurrentView = null;
            }

            return this;
        }

        /// <summary>
        /// Items Per Page.
        /// </summary>
        public int ItemsPerPage { get; private set; } = 20;

        /// <summary>
        /// Total Pages.
        /// </summary>
        public int TotalPages => m_TotalRecords / ItemsPerPage + ((m_TotalRecords % ItemsPerPage) > 0 ? 1 : 0);

        /// <summary>
        /// Page Number.
        /// </summary>
        public int PageNumber => Math.Min(Math.Max(m_PageIndex + 1, 1), TotalPages);

        /// <summary>
        /// Make Prepage enumerables.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> MakePrepages()
        {
            int First = Math.Max(1, PageNumber - 3);
            for (int i = First; i < PageNumber; ++i)
                yield return i;
        }

        /// <summary>
        /// Make Postpage enumerables.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> MakePostpages()
        {
            int Last = Math.Min(Math.Max(1, PageNumber + 3), TotalPages);
            for (int i = PageNumber + 1; i <= Last; ++i)
                yield return i;
        }

        /// <summary>
        /// Load View.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TEntity> LoadView()
        {
            if (m_CurrentView is null) /* Loads the Current View records. */
                m_CurrentView = m_Repository.LoadAsync(m_Query.Skip(m_PageIndex * ItemsPerPage).Take(ItemsPerPage)).Result;

            return m_CurrentView;
        }

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TEntity> GetEnumerator() => LoadView().GetEnumerator();

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => LoadView().GetEnumerator();
    }
}
