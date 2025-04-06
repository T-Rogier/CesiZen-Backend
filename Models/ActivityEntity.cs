namespace CesiZen_Backend.Models
{
    public sealed class Activity : EntityBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Content { get; private set; }
        public string ThumbnailImageLink { get; private set; }
        public TimeSpan EstimatedDuration { get; private set; }
        public int ViewCount { get; private set; }
        public bool Activated { get; private set; }
        public bool Deleted { get; private set; }
        public int CreatedById { get; private set; }
        public User CreatedBy { get; private set; }
        public ICollection<Category> Categories { get; private set; }
        public ActivityType Type { get; private set; }
        public ICollection<Participation> Participations { get; private set; } = new List<Participation>();
        public ICollection<SavedActivity> SavedActivities { get; private set; } = new List<SavedActivity>();


        private Activity()
        {
            Title = string.Empty;
            Description = string.Empty;
            Content = string.Empty;
            ThumbnailImageLink = string.Empty;
            EstimatedDuration = TimeSpan.Zero;
            ViewCount = 0;
            Activated = false;
            Deleted = false;
            CreatedBy = null!;
            Categories = [];
        }

        private Activity(string title, string description, string content, string thumbnailImageLink, TimeSpan estimatedDuration, User createdBy, ICollection<Category> categories, ActivityType type, bool activated = true)
        {
            Title = title;
            Description = description;
            Content = content;
            ThumbnailImageLink = thumbnailImageLink;
            EstimatedDuration = estimatedDuration;
            ViewCount = 0;
            Activated = activated;
            Deleted = false;
            CreatedBy = createdBy;
            CreatedById = createdBy.Id;
            Categories = categories;
            Type = type;
        }

        public static Activity Create(string title, string description, string content, string thumbnailImageLink, TimeSpan estimatedDuration, User createdBy, ICollection<Category> categories, ActivityType type, bool activated = true)
        {
            ValidateInputs(title, description, content, thumbnailImageLink, estimatedDuration, categories);
            return new Activity(title, description, content, thumbnailImageLink, estimatedDuration, createdBy, categories, type, activated);
        }

        public void Update(string title, string description, string content, string thumbnailImageLink, TimeSpan estimatedDuration, ICollection<Category> categories, bool activated, bool deleted)
        {
            ValidateInputs(title, description, content, thumbnailImageLink, estimatedDuration, categories);

            Title = title;
            Description = description;
            Content = content;
            ThumbnailImageLink = thumbnailImageLink;
            EstimatedDuration = estimatedDuration;
            Activated = activated;
            Deleted = deleted;
            Categories = categories;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string description, string content, string thumbnailImageLink, TimeSpan estimatedDuration, ICollection<Category> categories)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));

            if (string.IsNullOrWhiteSpace(thumbnailImageLink))
                throw new ArgumentException("ThumbnailImageLink cannot be null or empty.", nameof(thumbnailImageLink));

            if (estimatedDuration < TimeSpan.Zero)
                throw new ArgumentException("EstimatedDuration cannot be negative.", nameof(estimatedDuration));

            if (categories.Count > 3)
                throw new ArgumentException("An activity cannot have more than 3 categories.");

        }
    }
}
