using LuxAsp.Authorizations;
using LuxAsp.Notations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    [Table("LuxTokens")]
    public class LuxTokenModel : DataModel
    {
        protected const string INDEX_USER_BY_TOKEN = "user_by_token";

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
        /// Expiration Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Latest Refresh Time.
        /// </summary>
        [UniversalDateTime]
        public DateTime LatestRefreshTime { get; set; }

        /// <summary>
        /// Refresh Token Id.
        /// </summary>
        public Guid RefreshId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Refresh Expiration that increased when the refresh granted.
        /// </summary>
        [TimeSpanInSeconds]
        public TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromDays(7);

        /// <summary>
        /// User Id.
        /// </summary>
        [SearchIndexKey(INDEX_USER_BY_TOKEN, 0)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Test whether this token is expired or not.
        /// </summary>
        [NotMapped]
        public bool IsExpired => IsValid && ExpirationTime <= DateTime.Now;

        /// <summary>
        /// Refresh the token when this token saved.
        /// </summary>
        /// <param name="RefreshId"></param>
        /// <returns></returns>
        public bool Refresh(Guid RefreshId)
        {
            if (this.RefreshId != RefreshId)
                return false;

            this.RefreshId = Guid.NewGuid();
            LatestRefreshTime = DateTime.Now;
            ExpirationTime = DateTime.Now.Add(RefreshExpiration);
            return true;
        }

        /// <summary>
        /// Resolve the User Model who this token authorizes to user agent (aka, browser).
        /// When failed to resolve the user model, it means that, the token expired or the user deleted.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <returns></returns>
        public TUserModel Resolve<TUserModel>() where TUserModel : LuxUserModel
        {
            var Users = Services.GetRequiredService<Repository<TUserModel>>();
            return Users.Load(X => X.Where(Y => Y.Id == UserId)).FirstOrDefault();
        }
    }
}
