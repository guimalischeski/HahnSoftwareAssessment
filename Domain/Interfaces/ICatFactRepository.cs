using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICatFactRepository
    {
        Task<CatFact?> GetByIdAsync(Guid id);
        Task<List<CatFact>> GetAllAsync();
        Task AddAsync(CatFact catFact);
        Task UpdateAsync(CatFact catFact);
        Task DeleteAsync(CatFact catFact);
        Task<bool> ExistsAsync(string fact);
    }
}
