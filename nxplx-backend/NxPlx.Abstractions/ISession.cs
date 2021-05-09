namespace NxPlx.Abstractions
{
    public interface ISession
    {
        public int UserId { get; }
        public bool IsAdmin { get; }
        public string UserAgent { get; }
    }
}