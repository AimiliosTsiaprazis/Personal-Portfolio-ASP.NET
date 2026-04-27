using PortfolioApp.Models;

namespace PortfolioApp.Repositories
{
    public interface ISkillRepository
    {
        Task<IEnumerable<Skill>> GetAllAsync();
        Task<Skill?> GetByIdAsync(int id);
        Task<int> CreateAsync(Skill skill);
        Task<bool> UpdateAsync(Skill skill);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
