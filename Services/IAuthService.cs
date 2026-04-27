namespace PortfolioApp.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    }
}
