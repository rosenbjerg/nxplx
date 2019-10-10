namespace NxPlx.WebApi.Routers
{
    class Broadcast<T>
    {
        public Broadcast(string @event, T data)
        {
            Event = @event;
            Data = data;
        }
        public string Event { get; set; }
        public T Data { get; set; }
    }
}