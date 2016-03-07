using System;

namespace US.OpenServer.Protocols.DatabaseAuth
{
    public class DatabaseAuthProtocolException : Exception
    {
        public DatabaseAuthProtocolCommands ErrorCode { get; private set; }

        public DatabaseAuthProtocolException(DatabaseAuthProtocolCommands errorCode)
            : base ()
        {
            ErrorCode = errorCode;
        }
    }
}
