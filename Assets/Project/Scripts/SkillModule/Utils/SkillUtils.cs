namespace SkillModule.Utils
{
    public static class SkillUtils
    {
        /// <summary>
        /// 获取当前时间（秒）
        /// </summary>
        public static float GetCurrentTime()
        {
            return System.DateTime.Now.Ticks / 10000000f;
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        public static float CalculateDistance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return System.MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 规范化角度到 0-360 度
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }

        /// <summary>
        /// 检查值是否在范围内
        /// </summary>
        public static bool IsInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 生成唯一ID
        /// </summary>
        private static int idCounter = 0;
        public static int GenerateUniqueId()
        {
            return System.Threading.Interlocked.Increment(ref idCounter);
        }
    }
} 