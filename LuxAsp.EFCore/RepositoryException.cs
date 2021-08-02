using System;

namespace LuxAsp
{
    /// <summary>
    /// Repository related errors.
    /// </summary>
    public class RepositoryException : Exception
    {
        public RepositoryException(string Message) 
            : base(Message)
        {
        }
    }
}
