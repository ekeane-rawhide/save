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

        public static string GetHash(string password)
        {
            using var hasher = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(hasher.ComputeHash(bytes));
        }

        public async Task<Guid> InsertAsync(User user, bool rollback = false)
        {
            try
            {
                tblUser row = Map<User, tblUser>(user);
                row.Password = GetHash(user.Password);
                return await base.InsertAsync(row, e => e.UserId == user.UserId, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(User user, bool rollback = false)
        {
            try
            {
                tblUser row = Map<User, tblUser>(user);
                return await base.UpdateAsync(row, e => e.UserId == user.UserId, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<User>> LoadAsync()
        {
            try
            {
                var rows = new List<User>();
                (await base.LoadAsync()).ForEach(e => rows.Add(Map<tblUser, User>(e)));
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<User> LoadByIdAsync(Guid id)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault()
                          ?? throw new Exception("User not found.");
                return Map<tblUser, User>(row);
            }
            catch (Exception) { throw; }
        }

        public async Task<User> LoginAsync(string userId, string password)
        {
            try
            {
                using var dc = new SaveEntities(options);
                var hashed = GetHash(password);
                var row    = dc.tblUsers.FirstOrDefault(u => u.UserId == userId && u.Password == hashed)
                             ?? throw new LoginFailureException();

                // stamp last login
                row.LastLogin = DateTime.Now;
                dc.SaveChanges();

                return Map<tblUser, User>(row);
            }
            catch (Exception) { throw; }
        }
    }
}
