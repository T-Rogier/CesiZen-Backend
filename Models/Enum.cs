namespace CesiZen_Backend.Models
{
    public enum ActivityType
    {
        Classique,
        Ecriture,
        Jeu,
        Playlist
    }

    public enum UserRole
    {
        Admin,
        User
    }

    public enum SavedActivityStates
    {
        NoProgress,
        InProgress,
        Completed    
    }
}
