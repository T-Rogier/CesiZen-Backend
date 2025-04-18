namespace CesiZen_Backend.Models
{
    public sealed class User : EntityBase
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool Disabled { get; private set; }
        public UserRole Role { get; private set; }
        public ICollection<Participation> Participations { get; private set; } = new List<Participation>();
        public ICollection<SavedActivity> SavedActivities { get; private set; } = new List<SavedActivity>();

        private User()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            Disabled = false;
        }

        private User(string username, string email, string password, UserRole role, bool disabled = false)
        {
            Username = username;
            Email = email;
            Password = password;
            Disabled = disabled;
            Role = role;
        }

        public static User Create(string username, string email, string password, UserRole role, bool disabled = false)
        {
            ValidateInputs(username, email, password);
            return new User(username, email, password, role, disabled);
        }

        public void Update(string username, string email, string password, UserRole role, bool disabled)
        {
            ValidateInputs(username, email, password);
            Username = username;
            Email = email;
            Password = password;
            Disabled = disabled;
            Role = role;

            UpdateLastModified();
        }

        private static void ValidateInputs(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                throw new ArgumentException("Identifiant must be at least 3 characters long.", nameof(username));

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("Invalid email format.", nameof(email));

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(password));
        }
    }
}

