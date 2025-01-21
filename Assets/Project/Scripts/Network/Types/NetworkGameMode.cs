namespace Core.Network
{
    public enum NetworkGameMode : byte
    {
        None = 0,
        FreeForAll = 1,
        TeamDeathmatch = 2,
        CapturePoint = 3,
        Escort = 4,
        Custom = 255
    }
} 