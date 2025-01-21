namespace Core.Network
{
    public enum PlatformType
    {
        Standalone,
        Mobile,
        Console
    }

    public enum NetworkQuality
    {
        Excellent,
        Good,
        Fair,
        Poor,
        Critical
    }

    public enum ApplicationState
    {
        Foreground,
        Background,
        Paused,
        Resumed
    }

    [System.Flags]
    public enum NetworkCapabilities
    {
        None = 0,
        Wifi = 1 << 0,
        Ethernet = 1 << 1,
        Cellular = 1 << 2,
        BackgroundProcessing = 1 << 3,
        HighPerformance = 1 << 4,
        LowLatency = 1 << 5
    }

    public struct NetworkSettings
    {
        public int TargetFrameRate;
        public int QualityLevel;
        public bool EnableCompression;
        public float UpdateFrequency;
        public int MaxPacketSize;
        public bool AllowBackgroundProcessing;
    }
} 