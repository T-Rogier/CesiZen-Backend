namespace CesiZen_Backend.Models
{
    public abstract class EntityBase
    {
        public int Id { get; private init; }
        public DateTimeOffset Created { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset Updated { get; private set; } = DateTimeOffset.UtcNow;

        public void UpdateLastModified()
        {
            Updated = DateTimeOffset.UtcNow;
        }
    }
}
