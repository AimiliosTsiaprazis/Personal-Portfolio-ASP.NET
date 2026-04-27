using Microsoft.Data.SqlClient;

namespace PortfolioApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT PasswordHash FROM AdminUsers WHERE Username = @Username AND IsActive = 1",
                conn);
            cmd.Parameters.AddWithValue("@Username", username.Trim().ToLowerInvariant());

            var hash = await cmd.ExecuteScalarAsync() as string;
            if (string.IsNullOrEmpty(hash))
                return false;

            // Verify BCrypt hash – timing-safe comparison
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            if (!await ValidateUserAsync(username, currentPassword))
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "UPDATE AdminUsers SET PasswordHash = @Hash WHERE Username = @Username",
                conn);
            cmd.Parameters.AddWithValue("@Hash", newHash);
            cmd.Parameters.AddWithValue("@Username", username.Trim().ToLowerInvariant());
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
