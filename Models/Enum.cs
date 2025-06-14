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
        Administrateur,
        Utilisateur
    }

    public enum SavedActivityStates
    {
        NoProgress,
        InProgress,
        Completed    
    }
}
