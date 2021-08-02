using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxUserAuthenticationProvider<TUserModel> : ILuxAuthenticationProvider
        where TUserModel : LuxUserModel, new()
    {
        private static readonly Task<ILuxAuthenticatedMember> NULL_MEMBER
            = Task.FromResult(null as ILuxAuthenticatedMember);

        public LuxUserAuthenticationProvider(HttpRequest Request)
            => this.Request = Request;

        /// <summary>
        /// Http Request.
        /// </summary>
        public HttpRequest Request { get; }

        /// <summary>
        /// Resolve the Member from the User Repository.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public Task<ILuxAuthenticatedMember> ResolveAsync(Guid Guid, CancellationToken Cancellation = default)
        {
            var Repository = Request.GetRequiredService<Repository<TUserModel>>();
            var UserModel = Repository.Load(X => X.Where(X => X.Id == Guid)).FirstOrDefault();

            if (UserModel != null)
                return Task.FromResult<ILuxAuthenticatedMember>(UserModel.ToMember());

            return NULL_MEMBER;
        }
    }
}
