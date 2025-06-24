using System.ComponentModel.DataAnnotations;

namespace CesiZen_Backend.Models
{
    public enum ActivityType
    {
        [Display(Name = "Classique")]
        Classic,

        [Display(Name = "Écriture")]
        Writting,

        [Display(Name = "Jeu")]
        Game,

        [Display(Name = "Playlist")]
        Playlist
    }

    public enum UserRole
    {
        [Display(Name = "Administrateur")]
        Admin,

        [Display(Name = "Utilisateur")]
        User
    }

    public enum SavedActivityStates
    {
        [Display(Name = "Non commencé")]
        NoProgress,

        [Display(Name = "En cours")]
        InProgress,

        [Display(Name = "Terminé")]
        Completed
    }
}
