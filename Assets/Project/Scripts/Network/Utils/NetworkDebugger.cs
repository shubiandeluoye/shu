using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Linq;

namespace Network.Utils
{
    public class NetworkDebugger : MonoBehaviour
    {
        private NetworkRunner _runner;
        private bool _isEnabled;

        public void Initialize(NetworkRunner runner)
        {
            _runner = runner;
            _isEnabled = Debug.isDebugBuild;
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void LogNetworkState()
        {
            if (!_isEnabled) return;

            Debug.Log($"[NetworkDebugger] Runner State: {_runner.State}");
            Debug.Log($"[NetworkDebugger] Active Players: {_runner.ActivePlayers.Count()}");
            Debug.Log($"[NetworkDebugger] Is Server: {_runner.IsServer}");
            Debug.Log($"[NetworkDebugger] Is Connected: {_runner.IsConnectedToServer}");
        }

        public string GetStatisticsString()
        {
            if (_runner == null) return "No network statistics available";
            
            float deltaTime = (float)Time.deltaTime;  // 先转换为 float
            float frameTimeMs = deltaTime * 1000f;    // 然后进行乘法运算
            
            return $"Network Status:\n" +
                   $"RTT: {_runner.GetPlayerRtt(_runner.LocalPlayer):F1}ms\n" +
                   $"Simulation Time: {_runner.SimulationTime:F2}s\n" +
                   $"Frame Time: {frameTimeMs:F1}ms\n" +
                   $"Active Players: {_runner.ActivePlayers.Count()}";
        }

        public bool IsNetworkStable()
        {
            if (_runner == null) return false;
            
            float rtt = (float)_runner.GetPlayerRtt(_runner.LocalPlayer);  // 显式转换为 float
            return rtt < 200f;
        }
    }
} 