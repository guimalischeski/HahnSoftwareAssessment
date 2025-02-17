using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class CatFactRepository : ICatFactRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<CatFact> _dbSet;

        public CatFactRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<CatFact>();
        }

        public async Task<CatFact?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public async Task<List<CatFact>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<bool> ExistsAsync(string fact)
        {
            var hash = CatFact.ComputeHash(fact);
            return await _dbSet.AnyAsync(c => c.FactHash == hash);
        }

        public async Task AddAsync(CatFact catFact)
        {
            if (!await ExistsAsync(catFact.Fact))
            {
                await _dbSet.AddAsync(catFact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(CatFact catFact)
        {
            if (!await ExistsAsync(catFact.Fact))
            {
                _dbSet.Update(catFact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(CatFact catFact)
        {
            _dbSet.Remove(catFact);
            await _context.SaveChangesAsync();
        }
    }
}
