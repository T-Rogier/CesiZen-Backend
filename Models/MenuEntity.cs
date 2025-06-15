namespace CesiZen_Backend.Models
{
    public sealed class Menu : EntityBase
    {
        public string Title { get; private set; }
        public int HierarchyLevel { get; private set; }
        public int? ParentId { get; private set; }
        public Menu? Parent { get; private set; }
        public ICollection<Menu> Children { get; private set; } = new List<Menu>();
        public ICollection<Article> Articles { get; private set; } = new List<Article>();

        private Menu() 
        {
            Title = string.Empty;
        }

        public Menu(string title, int hierarchyLevel, int? parentId = null)
        {
            Title = title;
            HierarchyLevel = hierarchyLevel;
            ParentId = parentId;
        }

        public static Menu Create(string title, int hierarchyLevel, int? parentId = null)
        {
            ValidateInputs(title, hierarchyLevel);
            return new Menu(title, hierarchyLevel, parentId);
        }

        public void Update(string title, int hierarchyLevel, int? parentId = null)
        {
            ValidateInputs(title, hierarchyLevel);
            Title = title;
            HierarchyLevel = hierarchyLevel;
            ParentId = parentId;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, int hierarchyLevel)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (hierarchyLevel < 0)
                throw new ArgumentException("HierarchyLevel cannot be negative.", nameof(hierarchyLevel));
        }
    }
}
