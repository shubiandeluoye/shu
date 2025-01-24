namespace MapModule.Core
{
    public interface IMapManager
    {
        void PublishEvent<T>(string eventName, T eventData);
        void RegisterSystem(IMapSystem system);
    }

    public interface IMapSystem
    {
        void Initialize(IMapManager manager);
        void Dispose();
    }
} 