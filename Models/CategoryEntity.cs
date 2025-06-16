namespace CesiZen_Backend.Models
{
    public sealed class Category : EntityBase
    {
        public string Name { get; private set; }
        public string IconLink { get; private set; }
        public bool Deleted { get; private set; }

        private Category()
        {
            Name = string.Empty;
            IconLink = string.Empty;
            Deleted = false;
        }

        private Category(string name, string iconLink)
        {
            Name = name;
            IconLink = iconLink;
            Deleted = false;
        }

        public static Category Create(string name, string iconLink)
        {
            ValidateInputs(name, iconLink);
            return new Category(name, iconLink);
        }

        public void Update(string name, string iconLink)
        {
            ValidateInputs(name, iconLink);
            Name = name;
            IconLink = iconLink;
            UpdateLastModified();
        }

        private static void ValidateInputs(string name, string iconLink)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(iconLink))
                throw new ArgumentException("Icon link cannot be null or empty.", nameof(iconLink));
        }
    }
}
