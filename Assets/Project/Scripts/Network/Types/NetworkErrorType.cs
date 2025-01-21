namespace Core.Network
{
    public enum NetworkErrorType : byte
    {
        None = 0,
        ConnectionFailed = 1,
        Timeout = 2,
        ServerFull = 3,
        InvalidVersion = 4,
        AuthenticationFailed = 5,
        TeamFull = 6,
        InvalidOperation = 7,
        InternalError = 8
    }
} 