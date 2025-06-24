namespace CesiZen_Backend.Services.PasswordHandler
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string plain)
            => BCrypt.Net.BCrypt.HashPassword(plain);

        public bool Verify(string plain, string hash)
            => BCrypt.Net.BCrypt.Verify(plain, hash);
    }
}
