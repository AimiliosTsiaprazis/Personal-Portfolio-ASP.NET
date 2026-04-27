using Microsoft.Data.SqlClient;
using PortfolioApp.Models;

namespace PortfolioApp.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly string _connectionString;

        public SkillRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            var skills = new List<Skill>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Category, ProficiencyLevel, IconClass, SortOrder FROM Skills ORDER BY SortOrder, Name",
                conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                skills.Add(MapSkill(reader));
            return skills;
        }

        public async Task<Skill?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Category, ProficiencyLevel, IconClass, SortOrder FROM Skills WHERE Id = @Id",
                conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapSkill(reader);
            return null;
        }

        public async Task<int> CreateAsync(Skill skill)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                INSERT INTO Skills (Name, Category, ProficiencyLevel, IconClass, SortOrder)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Category, @ProficiencyLevel, @IconClass, @SortOrder)",
                conn);
            cmd.Parameters.AddWithValue("@Name", skill.Name);
            cmd.Parameters.AddWithValue("@Category", skill.Category);
            cmd.Parameters.AddWithValue("@ProficiencyLevel", skill.ProficiencyLevel);
            cmd.Parameters.AddWithValue("@IconClass", (object?)skill.IconClass ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SortOrder", skill.SortOrder);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Skill skill)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                UPDATE Skills SET
                    Name = @Name,
                    Category = @Category,
                    ProficiencyLevel = @ProficiencyLevel,
                    IconClass = @IconClass,
                    SortOrder = @SortOrder
                WHERE Id = @Id",
                conn);
            cmd.Parameters.AddWithValue("@Id", skill.Id);
            cmd.Parameters.AddWithValue("@Name", skill.Name);
            cmd.Parameters.AddWithValue("@Category", skill.Category);
            cmd.Parameters.AddWithValue("@ProficiencyLevel", skill.ProficiencyLevel);
            cmd.Parameters.AddWithValue("@IconClass", (object?)skill.IconClass ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SortOrder", skill.SortOrder);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("DELETE FROM Skills WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categories = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT DISTINCT Category FROM Skills ORDER BY Category",
                conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                categories.Add(reader.GetString(0));
            return categories;
        }

        private static Skill MapSkill(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            Category = r.GetString(2),
            ProficiencyLevel = r.GetInt32(3),
            IconClass = r.IsDBNull(4) ? null : r.GetString(4),
            SortOrder = r.GetInt32(5)
        };
    }
}
