namespace CesiZen_Backend.Models
{
    public sealed class Participation
    {
        public int Id { get; private init; }
        public int UserId { get; private set; }
        public User User { get; private set; }

        public int ActivityId { get; private set; }
        public Activity Activity { get; private set; }

        public DateTime ParticipationDate { get; private set; }
        public TimeSpan Duration { get; private set; }

        private Participation()
        {
            User = null!;
            Activity = null!;
            ParticipationDate = DateTime.UtcNow;
            Duration = TimeSpan.Zero;
        }

        public Participation(User user, Activity activity, DateTime date, TimeSpan duration)
        {
            UserId = user.Id;
            User = user;
            ActivityId = activity.Id;
            Activity = activity;
            ParticipationDate = date;
            Duration = duration;
        }
        public static Participation Create(User user, Activity activity, DateTime date, TimeSpan duration)
        {
            return new Participation(user, activity, date, duration);
        }
    }
}
