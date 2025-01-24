namespace MapModule.Core.Utils
{
    public static class TimeUtils
    {
        private static float currentTime;
        private static float deltaTime;

        public static float GetCurrentTime() => currentTime;
        public static float GetDeltaTime() => deltaTime;

        public static void Update(float time, float delta)
        {
            currentTime = time;
            deltaTime = delta;
        }
    }
} 