namespace VtConnect.Exceptions
{
    using System;

    public class UnhandledCredentialTypeException : VtConnectException
    {
        public UnhandledCredentialTypeException(string message) :
            base(message)
        {
        }

        public UnhandledCredentialTypeException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
