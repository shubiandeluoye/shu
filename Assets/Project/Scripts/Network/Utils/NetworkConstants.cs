namespace Core.Network
{
    public static class NetworkConstants
    {
        // 网络配置
        public const int MAX_PLAYERS = 32;
        public const int MAX_TEAMS = 4;
        public const int MAX_ROOMS = 100;
        
        // 游戏配置
        public const float TICK_RATE = 60f;
        public const float INTERPOLATION_DELAY = 0.1f;
        public const float MAX_EXTRAPOLATION_TIME = 0.5f;
        
        // 同步阈值
        public const float POSITION_THRESHOLD = 0.01f;
        public const float ROTATION_THRESHOLD = 0.1f;
        public const float VELOCITY_THRESHOLD = 0.01f;
        
        // 超时设置
        public const float CONNECTION_TIMEOUT = 10f;
        public const float ROOM_TIMEOUT = 300f;
        public const float MATCH_TIMEOUT = 3600f;
        
        // 缓冲区大小
        public const int INPUT_BUFFER_SIZE = 32;
        public const int STATE_BUFFER_SIZE = 64;
        
        // 网络层级
        public const int NETWORK_SCENE_INDEX = 1;
        public const string NETWORK_SCENE_NAME = "NetworkScene";
    }
} 