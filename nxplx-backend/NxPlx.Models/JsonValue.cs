namespace NxPlx.Models
{
    public class JsonValue<T>
    {
        public T value { get; set; }
    }
    public class JsonValue<T1, T2>
    {
        public T1 value1 { get; set; }
        public T2 value2 { get; set; }
    }
}