namespace CesiZen_Backend.Models
{
    public sealed class Article : EntityBase
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int MenuId { get; private set; }
        public Menu Menu { get; private set; }

        private Article()
        {
            Title = string.Empty;
            Content = string.Empty;
            Menu = null!;
        }

        public Article(string title, string content, Menu menu)
        {
            Title = title;
            Content = content;
            MenuId = menu.Id;
            Menu = menu;
        }

        public static Article Create(string title, string content, Menu menu)
        {
            ValidateInputs(title, content);
            return new Article(title, content, menu);
        }

        public void Update(string title, string content, Menu menu)
        {
            ValidateInputs(title, content);

            Title = title;
            Content = content;
            MenuId = menu.Id;
            Menu = menu;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        }
    }
}

