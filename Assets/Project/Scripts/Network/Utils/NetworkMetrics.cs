using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

namespace Network.Utils
{
    /// <summary>
    /// 网络性能监控组件
    /// 监控延迟、丢包率、带宽等网络性能指标
    /// </summary>
    public class NetworkMetrics : NetworkBehaviour
    {
        private NetworkRunner runner;

        private struct MetricsData
        {
            public float RoundTripTime;    // 往返时间
            public float PacketLoss;       // 丢包率
            public int Fps;                // 帧率
            public float BytesSentPerSecond;    // 每秒发送字节数
            public float BytesReceivedPerSecond; // 每秒接收字节数
            public int ActivePlayers;      // 活跃玩家数
        }

        private MetricsData currentMetrics;
        private float updateInterval = 1.0f;
        private float nextUpdateTime;

        private void Awake()
        {
            // 在 Awake 中获取 NetworkRunner
            runner = GetComponentInParent<NetworkRunner>();
        }

        public override void FixedUpdateNetwork()
        {
            if (Time.time >= nextUpdateTime)
            {
                UpdateMetrics();
                nextUpdateTime = Time.time + updateInterval;
            }
        }

        private void UpdateMetrics()
        {
            if (!runner || !runner.IsRunning) return;

            try 
            {
                currentMetrics = new MetricsData
                {
                    RoundTripTime = (float)runner.GetPlayerRtt(runner.LocalPlayer),
                    PacketLoss = 0f,
                    Fps = (int)(1.0f / Time.deltaTime),
                    BytesSentPerSecond = 0f,
                    BytesReceivedPerSecond = 0f,
                    ActivePlayers = runner.ActivePlayers.Count()
                };

                CheckNetworkPerformance();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NetworkMetrics] Failed to update metrics: {e.Message}");
            }
        }

        private void CheckNetworkPerformance()
        {
            if (currentMetrics.PacketLoss > 0.05f)
            {
                Debug.LogWarning($"[NetworkMetrics] 高丢包率警告: {currentMetrics.PacketLoss:P1}");
            }

            if (currentMetrics.RoundTripTime > 200)
            {
                Debug.LogWarning($"[NetworkMetrics] 高延迟警告: {currentMetrics.RoundTripTime:F0}ms");
            }

            if (currentMetrics.Fps < 30)
            {
                Debug.LogWarning($"[NetworkMetrics] 低帧率警告: {currentMetrics.Fps} FPS");
            }
        }

        /// <summary>
        /// 获取当前网络性能报告
        /// </summary>
        public string GetMetricsReport()
        {
            if (!runner || !runner.IsRunning)
                return "Network not running";

            return $"Network Performance Report:\n" +
                   $"RTT (延迟): {currentMetrics.RoundTripTime:F0}ms\n" +
                   $"Packet Loss (丢包率): {currentMetrics.PacketLoss:P1}\n" +
                   $"FPS (帧率): {currentMetrics.Fps}\n" +
                   $"Upload (上传): {currentMetrics.BytesSentPerSecond / 1024:F1} KB/s\n" +
                   $"Download (下载): {currentMetrics.BytesReceivedPerSecond / 1024:F1} KB/s\n" +
                   $"Active Players (在线玩家): {currentMetrics.ActivePlayers}";
        }

        /// <summary>
        /// 获取简短的性能状态
        /// </summary>
        public string GetShortStatus()
        {
            if (!runner || !runner.IsRunning)
                return "Offline";

            return $"RTT: {currentMetrics.RoundTripTime:F0}ms | " +
                   $"Loss: {currentMetrics.PacketLoss:P0} | " +
                   $"FPS: {currentMetrics.Fps}";
        }

        /// <summary>
        /// 检查网络连接是否健康
        /// </summary>
        public bool IsNetworkHealthy()
        {
            return runner != null && 
                   runner.IsRunning && 
                   currentMetrics.PacketLoss < 0.05f && 
                   currentMetrics.RoundTripTime < 200;
        }

        // Simulate 相关方法
        public float GetSimulationTime()
        {
            if (runner == null) return 0f;
            return runner.SimulationTime;
        }

        // GetPlayerObject 相关方法
        public NetworkObject GetPlayerObject(PlayerRef player)
        {
            return runner.GetPlayerObject(player);
        }
    }
} 