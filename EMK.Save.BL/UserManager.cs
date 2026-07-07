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
                string hashed   = GetHash(password);
                tblUser row     = dc.tblUsers
                                    .FirstOrDefault(u => u.UserId == userId && u.Password == hashed)
                                  ?? throw new LoginFailureException();
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
