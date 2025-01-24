using System.Collections.Generic;

namespace MapModule.Core.Systems
{
    public class MapManager : IMapManager
    {
        private static MapManager instance;
        public static MapManager Instance => instance ??= new MapManager();

        private readonly MapEventSystem eventSystem;
        private readonly Dictionary<int, ShapeState> shapes = new Dictionary<int, ShapeState>();
        private readonly List<IMapSystem> systems = new List<IMapSystem>();

        private MapManager()
        {
            eventSystem = MapEventSystem.Instance;
        }

        public void Initialize(MapConfig config)
        {
            // 初始化各个系统
            var centralAreaSystem = new CentralAreaSystem(config, this);
            var shapeSystem = new ShapeSystem(config, this);

            RegisterSystem(centralAreaSystem);
            RegisterSystem(shapeSystem);
        }

        public void PublishEvent<T>(string eventName, T eventData)
        {
            eventSystem.Publish(eventName, eventData);
        }

        public void RegisterSystem(IMapSystem system)
        {
            systems.Add(system);
            system.Initialize(this);
        }

        public void Dispose()
        {
            foreach (var system in systems)
            {
                system.Dispose();
            }
            systems.Clear();
            shapes.Clear();
            eventSystem.Clear();
        }
    }
} 