using PortfolioApp.Models;

namespace PortfolioApp.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<int> CreateAsync(Project project);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Project>> GetFeaturedAsync();
    }
}
