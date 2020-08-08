namespace NxPlx.Application.Core
{
    public class Message
    {
        public Message(string @event, object data)
        {
            Event = @event;
            Data = data;
        }

        public string Event { get; set; }
        public object Data { get; set; }
    }
}