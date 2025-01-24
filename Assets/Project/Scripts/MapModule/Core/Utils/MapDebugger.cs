using System;
using System.Collections.Generic;

namespace MapModule.Core.Utils
{
    public class MapDebugger
    {
        private static bool isDebugEnabled;
        private static readonly List<string> logBuffer = new();
        public static event Action<string> OnLogMessage;

        public static void EnableDebug(bool enable)
        {
            isDebugEnabled = enable;
        }

        public static void LogShapeInfo(string message, ShapeType type, Vector3D position)
        {
            if (!isDebugEnabled) return;
            var logMessage = $"[Map][Shape:{type}] {message} at ({position.X}, {position.Y}, {position.Z})";
            logBuffer.Add(logMessage);
            OnLogMessage?.Invoke(logMessage);
        }

        public static void ClearLogs()
        {
            logBuffer.Clear();
        }

        public static IReadOnlyList<string> GetLogs() => logBuffer;
    }
} 