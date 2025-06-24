namespace CesiZen_Backend.Services.PasswordHandler
{
    public interface IPasswordHasher
    {
        string HashPassword(string plain);
        bool Verify(string plain, string hash);
    }
}
