namespace Core.Network
{
    public enum NetworkActionType : byte
    {
        None = 0,
        Attack = 1,
        Block = 2,
        Dodge = 3,
        UseItem = 4,
        Interact = 5,
        Emote = 6,
        Ping = 7,
        Custom = 255
    }
} 