namespace SistemaCertificaion.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentException("Password cannot be null");
            }
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (password == null)
            {
                throw new ArgumentException("Password cannot be null");
            }

            if (hashedPassword == null)
            {
                throw new ArgumentException("Password cannot be null");
            }
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

    
    
}
