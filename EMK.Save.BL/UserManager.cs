namespace EMK.Save.BL
{
    public class LoginFailureException : Exception
    {
        public LoginFailureException() : base("Cannot log in with these credentials.") { }
        public LoginFailureException(string message) : base(message) { }
    }

    public class UserManager : GenericManager<tblUser>
    {
        public UserManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }
        public UserManager(DbContextOptions<SaveEntities> options) : base(options) { }

        // ── Password hashing — PBKDF2-HMAC-SHA256, self-describing "iterations.salt.hash" format ──
        private const int Pbkdf2Iterations = 210_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, HashSize);
            return $"{Pbkdf2Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string stored)
        {
            string[] parts = stored.Split('.');
            if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations))
                return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expected = Convert.FromBase64String(parts[2]);
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        public async Task<Guid> InsertAsync(User user, bool rollback = false)
        {
            try
            {
                tblUser row = Map<User, tblUser>(user);
                row.Password = HashPassword(user.Password);
                return await base.InsertAsync(row,
                    e => e.UserId == user.UserId,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(User user, bool rollback = false)
        {
            try
            {
                tblUser row = Map<User, tblUser>(user);
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<User>> LoadAsync()
        {
            try
            {
                var rows = new List<User>();
                (await base.LoadAsync())
                    .ForEach(e =>
                    {
                        var u = Map<tblUser, User>(e);
                        u.BudgetRole = e.BudgetRole.HasValue ? (BudgetRole?)e.BudgetRole.Value : null;
                        rows.Add(u);
                    });
                return rows;
            }
            catch (Exception) { throw; }
        }

        public new async Task<User> LoadByIdAsync(Guid id)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault()
                          ?? throw new Exception("User not found.");
                var u = Map<tblUser, User>(row);
                u.BudgetRole = row.BudgetRole.HasValue ? (BudgetRole?)row.BudgetRole.Value : null;
                return u;
            }
            catch (Exception) { throw; }
        }

        public async Task<User> LoginAsync(string userId, string password)
        {
            try
            {
                using var dc    = new SaveEntities(options);
                tblUser row     = dc.tblUsers.FirstOrDefault(u => u.UserId == userId)
                                  ?? throw new LoginFailureException();

                if (!VerifyPassword(password, row.Password))
                    throw new LoginFailureException();

                row.LastLogin   = DateTime.Now;
                dc.SaveChanges();

                var u = Map<tblUser, User>(row);
                u.BudgetRole = row.BudgetRole.HasValue ? (BudgetRole?)row.BudgetRole.Value : null;
                return u;
            }
            catch (Exception) { throw; }
        }
    }
}
