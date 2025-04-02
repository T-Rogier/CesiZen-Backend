namespace CesiZen_Backend.Models
{
    public sealed class Activity : EntityBase
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Content { get; private set; }
        public string ThumbnailImageLink { get; private set; }

        private Activity()
        {
            Title = string.Empty;
            Description = string.Empty;
            Content = string.Empty;
            ThumbnailImageLink = string.Empty;
        }
        private Activity(string title, string description, string content, string thumbnailImageLink)
        {
            Title = title;
            Description = description;
            Content = content;
            ThumbnailImageLink = thumbnailImageLink;
        }

        public static Activity Create(string title, string description, string content, string thumbnailImageLink)
        {
            ValidateInputs(title, description, content, thumbnailImageLink);
            return new Activity(title, description, content, thumbnailImageLink);
        }

        public void Update(string title, string description, string content, string thumbnailImageLink)
        {
            ValidateInputs(title, description, content, thumbnailImageLink);

            Title = title;
            Description = description;
            Content = content;
            ThumbnailImageLink = thumbnailImageLink;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string description, string content, string thumbnailImageLink)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Genre cannot be null or empty.", nameof(description));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Genre cannot be null or empty.", nameof(content));

            if (string.IsNullOrWhiteSpace(thumbnailImageLink))
                throw new ArgumentException("Genre cannot be null or empty.", nameof(thumbnailImageLink));
        }
    }
}
