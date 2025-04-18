namespace CesiZen_Backend.Models
{
    public sealed class SavedActivity
    {
        public int UserId { get; private set; }
        public User User { get; private set; }

        public int ActivityId { get; private set; }
        public Activity Activity { get; private set; }

        public bool IsFavoris { get; private set; }
        public SavedActivityStates State { get; private set; }
        public Percentage Progress { get; private set; }

        private SavedActivity()
        {
            User = null!;
            Activity = null!;
            IsFavoris = false;
            State = SavedActivityStates.NoProgress;
            Progress = new Percentage(0);
        }

        public SavedActivity(User user, Activity activity, bool isFavoris, SavedActivityStates state, Percentage progress)
        {
            UserId = user.Id;
            User = user;
            ActivityId = activity.Id;
            Activity = activity;
            IsFavoris = isFavoris;
            State = state;
            Progress = progress;
        }

        public static SavedActivity Create(User user, Activity activity, bool isFavoris, SavedActivityStates state, Percentage progress)
        {
            return new SavedActivity(user, activity, isFavoris, state, progress);
        }

        public void Update(bool isFavoris, SavedActivityStates state, Percentage progress)
        {
            IsFavoris = isFavoris;
            State = state;
            Progress = progress;
        }
    }
}
