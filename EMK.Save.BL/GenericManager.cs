namespace EMK.Save.BL
{
    public abstract class GenericManager<T> where T : class, IEntity
    {
        protected DbContextOptions<SaveEntities> options;
        protected readonly ILogger? logger;

        public GenericManager(DbContextOptions<SaveEntities> options)
        {
            this.options = options;
        }

        public GenericManager(DbContextOptions<SaveEntities> options, ILogger logger)
        {
            this.options = options;
            this.logger = logger;
        }

        // ── Reflection-based mapper (name + type match) ───────────────────────
        public V Map<U, V>(U objfrom)
        {
            V objTo = (V)Activator.CreateInstance(typeof(V))!;

            var toProps   = objTo?.GetType().GetProperties();
            var fromProps = objfrom?.GetType().GetProperties();

            toProps?.ToList().ForEach(o =>
            {
                var fromp = fromProps?.FirstOrDefault(x => x.Name == o.Name
                                                        && x.PropertyType == o.PropertyType);
                if (fromp != null)
                    o.SetValue(objTo, fromp.GetValue(objfrom));
            });

            return objTo;
        }

        // ── Load ──────────────────────────────────────────────────────────────
        public async Task<List<T>> LoadAsync(
            Expression<Func<T, bool>>?        filter            = null,
            Expression<Func<T, object>>[]?    includeProperties = null)
        {
            try
            {
                if (filter == null) filter = e => true;

                IQueryable<T> rows = new SaveEntities(options).Set<T>().Where(filter);

                if (includeProperties != null)
                    foreach (var prop in includeProperties)
                        rows = rows.Include(prop);

                var result = rows.ToList();
                logger?.LogInformation("Load {Entity}: {Count} records", typeof(T).Name, result.Count);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> LoadByIdAsync(Guid id)
        {
            try
            {
                return (await LoadAsync(e => e.Id == id)).FirstOrDefault()!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ── Insert ────────────────────────────────────────────────────────────
        public async Task<Guid> InsertAsync(
            T                             entity,
            Expression<Func<T, bool>>?    duplicateCheck = null,
            bool                          rollback       = false)
        {
            try
            {
                using SaveEntities dc = new SaveEntities(options);
                IDbContextTransaction? transaction = null;
                if (rollback) transaction = dc.Database.BeginTransaction();

                entity.Id = Guid.NewGuid();

                if (duplicateCheck != null)
                {
                    var existing = dc.Set<T>().FirstOrDefault(duplicateCheck);
                    if (existing != null) throw new Exception($"Duplicate {typeof(T).Name} detected.");
                }

                dc.Set<T>().Add(entity);
                dc.SaveChanges();

                if (rollback) transaction?.Rollback();

                return entity.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ── Update ────────────────────────────────────────────────────────────
        public async Task<int> UpdateAsync(
            T                             entity,
            Expression<Func<T, bool>>?    duplicateCheck = null,
            bool                          rollback       = false)
        {
            try
            {
                int results = 0;
                using SaveEntities dc = new SaveEntities(options);
                IDbContextTransaction? transaction = null;
                if (rollback) transaction = dc.Database.BeginTransaction();

                if (duplicateCheck != null)
                {
                    var existing = dc.Set<T>().FirstOrDefault(duplicateCheck);
                    if (existing != null && existing.Id != entity.Id)
                        throw new Exception($"Duplicate {typeof(T).Name} detected.");
                }

                dc.Set<T>().Update(entity);
                results = dc.SaveChanges();

                if (rollback) transaction?.Rollback();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ── Delete ────────────────────────────────────────────────────────────
        public async Task<int> DeleteAsync(Guid id, bool rollback = false)
        {
            try
            {
                int results = 0;
                using SaveEntities dc = new SaveEntities(options);
                IDbContextTransaction? transaction = null;
                if (rollback) transaction = dc.Database.BeginTransaction();

                T row = dc.Set<T>().FirstOrDefault(t => t.Id == id)
                        ?? throw new Exception("Row does not exist.");

                dc.Set<T>().Remove(row);
                results = dc.SaveChanges();

                if (rollback) transaction?.Rollback();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
