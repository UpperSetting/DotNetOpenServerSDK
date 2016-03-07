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

        public override string Message
        {
            get
            {
                switch (ErrorCode)
                {
                    case DatabaseAuthProtocolCommands.ACCESS_DENIED:
                        return DatabaseAuthProtocol.ERROR_ACCESS_DENIED;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED_EMAIL_NOT_VERIFIED:
                        return DatabaseAuthProtocol.ERROR_NOT_VERIFIED;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED_INVALID_PASSWORD:
                        return DatabaseAuthProtocol.ERROR_USER_NOT_FOUND;
                }
                return base.Message;
            }
        }
    }
}
