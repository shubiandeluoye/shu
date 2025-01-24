namespace GameModule.Core.Events
{
    public interface IEventSystem
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
    }
} 