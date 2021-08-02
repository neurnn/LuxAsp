namespace LuxAsp.Authorizations
{
    /// <summary>
    /// User Model Extensions.
    /// </summary>
    public static class LuxUserModelExtensions
    {
        /// <summary>
        /// Make the ILuxAuthenticatedMember instance from the UserModel instance.
        /// </summary>
        /// <typeparam name="TUserModel"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxAuthenticatedMember<TUserModel> ToMember<TUserModel>(this TUserModel This)
            where TUserModel : LuxUserModel => LuxAuthenticatedMember<TUserModel>.FromModel(This.Id, This);
    }
}
