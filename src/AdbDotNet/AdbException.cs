namespace Vurdalakov
{
    using System;

    public class AdbException : Exception
    {
        internal AdbException(String message) : base(message)
        {
        }
    }

    public class AdbInvalidResponseException : AdbException
    {
        internal AdbInvalidResponseException(String response) : base($"The server returned an invalid response '{response}'")
        {
        }
    }
}
