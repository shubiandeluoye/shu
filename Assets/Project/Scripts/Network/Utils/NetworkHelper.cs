using UnityEngine;
using Fusion;
using System.Collections.Generic;
using Core.Network; 

namespace Network.Utils
{
    public static class NetworkHelper
    {
        public static bool IsValidPosition(Vector3 position)
        {
            return !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z) &&
                   !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z);
        }

        public static bool IsValidRotation(Quaternion rotation)
        {
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && 
                   !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w);
        }

        public static Vector3 ClampPosition(Vector3 position, float maxDistance)
        {
            if (position.magnitude > maxDistance)
            {
                return position.normalized * maxDistance;
            }
            return position;
        }

        public static float GetInterpolationTime(NetworkRunner runner)
        {
            if (runner == null) return 0f;
            return runner.SimulationTime - NetworkConstants.INTERPOLATION_DELAY;
        }

        public static float GetExtrapolationTime(NetworkRunner runner)
        {
            if (runner == null) return 0f;
            return Mathf.Min(runner.SimulationTime + NetworkConstants.MAX_EXTRAPOLATION_TIME, 
                           runner.SimulationTime + runner.DeltaTime * 2);
        }

        public static bool ShouldInterpolate(NetworkRunner runner, float targetTime)
        {
            return runner != null && targetTime <= runner.SimulationTime;
        }

        public static float CalculateJitterBuffer(float averageLatency)
        {
            return Mathf.Max(averageLatency * 2f, NetworkConstants.INTERPOLATION_DELAY);
        }

        public static int GetOptimalTickRate(float averageLatency)
        {
            return Mathf.Max(30, Mathf.Min(60, Mathf.RoundToInt(1f / averageLatency)));
        }

        public static bool IsWithinThreshold(Vector3 a, Vector3 b, float threshold)
        {
            return Vector3.SqrMagnitude(a - b) <= threshold * threshold;
        }

        public static bool IsWithinThreshold(Quaternion a, Quaternion b, float threshold)
        {
            return Quaternion.Angle(a, b) <= threshold;
        }

        public static bool HasStateAuthority(NetworkRunner runner)
        {
            return runner != null && runner.IsServer;
        }

        public static bool HasInputAuthority(NetworkBehaviour behaviour)
        {
            return behaviour != null && behaviour.HasInputAuthority;
        }

        public static float GetSimulationTime(NetworkRunner runner)
        {
            return runner != null ? runner.SimulationTime : 0f;
        }

        public static float GetInterpolationFactor(float fromTime, float toTime, float currentTime)
        {
            return Mathf.Clamp01((currentTime - fromTime) / (toTime - fromTime));
        }

        public static void SetInterpolation(this NetworkRunner runner, SimulationConfig config)
        {
            if (runner != null && runner.IsRunning)
            {
                Debug.LogWarning("Network configuration can only be set during initialization");
            }
        }

        public static bool ShouldInterpolateState(NetworkRunner runner)
        {
            return runner != null && 
                   runner.Config != null &&
                   runner.IsClient;
        }
    }
} 