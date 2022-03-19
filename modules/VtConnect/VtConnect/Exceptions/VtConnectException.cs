namespace VtConnect.Exceptions
{
    using System;

    public class VtConnectException : Exception
    {
        public VtConnectException(string message) :
            base(message)
        {
        }

        public VtConnectException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
