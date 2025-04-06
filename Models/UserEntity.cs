namespace CesiZen_Backend.Models
{
    public sealed class User : EntityBase
    {
        public string Identifiant { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool Disabled { get; private set; }
        public UserRole Role { get; private set; }
        public ICollection<Participation> Participations { get; private set; } = new List<Participation>();
        public ICollection<SavedActivity> SavedActivities { get; private set; } = new List<SavedActivity>();

        private User()
        {
            Identifiant = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            Disabled = false;
        }

        private User(string identifiant, string email, string password, UserRole role, bool disabled = false)
        {
            Identifiant = identifiant;
            Email = email;
            Password = password;
            Disabled = disabled;
            Role = role;
        }

        public static User Create(string identifiant, string email, string password, UserRole role, bool disabled = false)
        {
            ValidateInputs(identifiant, email, password);
            return new User(identifiant, email, password, role, disabled);
        }

        public void Update(string identifiant, string email, string password, UserRole role, bool disabled)
        {
            ValidateInputs(identifiant, email, password);
            Identifiant = identifiant;
            Email = email;
            Password = password;
            Disabled = disabled;
            Role = role;

            UpdateLastModified();
        }

        private static void ValidateInputs(string identifiant, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(identifiant) || identifiant.Length < 3)
                throw new ArgumentException("Identifiant must be at least 3 characters long.", nameof(identifiant));

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("Invalid email format.", nameof(email));

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(password));
        }
    }
}

