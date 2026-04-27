using Microsoft.Data.SqlClient;
using PortfolioApp.Models;

namespace PortfolioApp.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            var projects = new List<Project>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Title, Description, TechStack, ProjectUrl, GitHubUrl, ImageUrl, IsFeatured, CreatedAt, UpdatedAt FROM Projects ORDER BY CreatedAt DESC",
                conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                projects.Add(MapProject(reader));
            return projects;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Title, Description, TechStack, ProjectUrl, GitHubUrl, ImageUrl, IsFeatured, CreatedAt, UpdatedAt FROM Projects WHERE Id = @Id",
                conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapProject(reader);
            return null;
        }

        public async Task<int> CreateAsync(Project project)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                INSERT INTO Projects (Title, Description, TechStack, ProjectUrl, GitHubUrl, ImageUrl, IsFeatured, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@Title, @Description, @TechStack, @ProjectUrl, @GitHubUrl, @ImageUrl, @IsFeatured, @CreatedAt)",
                conn);
            cmd.Parameters.AddWithValue("@Title", project.Title);
            cmd.Parameters.AddWithValue("@Description", project.Description);
            cmd.Parameters.AddWithValue("@TechStack", (object?)project.TechStack ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProjectUrl", (object?)project.ProjectUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GitHubUrl", (object?)project.GitHubUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)project.ImageUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFeatured", project.IsFeatured);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Project project)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                UPDATE Projects SET
                    Title = @Title,
                    Description = @Description,
                    TechStack = @TechStack,
                    ProjectUrl = @ProjectUrl,
                    GitHubUrl = @GitHubUrl,
                    ImageUrl = @ImageUrl,
                    IsFeatured = @IsFeatured,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id",
                conn);
            cmd.Parameters.AddWithValue("@Id", project.Id);
            cmd.Parameters.AddWithValue("@Title", project.Title);
            cmd.Parameters.AddWithValue("@Description", project.Description);
            cmd.Parameters.AddWithValue("@TechStack", (object?)project.TechStack ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProjectUrl", (object?)project.ProjectUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GitHubUrl", (object?)project.GitHubUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)project.ImageUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFeatured", project.IsFeatured);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("DELETE FROM Projects WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<Project>> GetFeaturedAsync()
        {
            var projects = new List<Project>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Title, Description, TechStack, ProjectUrl, GitHubUrl, ImageUrl, IsFeatured, CreatedAt, UpdatedAt FROM Projects WHERE IsFeatured = 1 ORDER BY CreatedAt DESC",
                conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                projects.Add(MapProject(reader));
            return projects;
        }

        private static Project MapProject(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Title = r.GetString(1),
            Description = r.GetString(2),
            TechStack = r.IsDBNull(3) ? null : r.GetString(3),
            ProjectUrl = r.IsDBNull(4) ? null : r.GetString(4),
            GitHubUrl = r.IsDBNull(5) ? null : r.GetString(5),
            ImageUrl = r.IsDBNull(6) ? null : r.GetString(6),
            IsFeatured = r.GetBoolean(7),
            CreatedAt = r.GetDateTime(8),
            UpdatedAt = r.IsDBNull(9) ? null : r.GetDateTime(9)
        };
    }
}
