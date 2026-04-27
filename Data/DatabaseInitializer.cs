using Microsoft.Data.SqlClient;

namespace PortfolioApp.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Creating Tables
            ExecuteNonQuery(conn, @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AdminUsers' AND xtype='U')
                CREATE TABLE AdminUsers (
                    Id              INT IDENTITY(1,1) PRIMARY KEY,
                    Username        NVARCHAR(100) NOT NULL UNIQUE,
                    PasswordHash    NVARCHAR(256) NOT NULL,
                    IsActive        BIT NOT NULL DEFAULT 1,
                    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    LastLoginAt     DATETIME2 NULL
                )");

            ExecuteNonQuery(conn, @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
                CREATE TABLE Projects (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    Title       NVARCHAR(150) NOT NULL,
                    Description NVARCHAR(1000) NOT NULL,
                    TechStack   NVARCHAR(300) NULL,
                    ProjectUrl  NVARCHAR(500) NULL,
                    GitHubUrl   NVARCHAR(500) NULL,
                    ImageUrl    NVARCHAR(500) NULL,
                    IsFeatured  BIT NOT NULL DEFAULT 0,
                    CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    UpdatedAt   DATETIME2 NULL
                )");

            ExecuteNonQuery(conn, @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Skills' AND xtype='U')
                CREATE TABLE Skills (
                    Id              INT IDENTITY(1,1) PRIMARY KEY,
                    Name            NVARCHAR(100) NOT NULL,
                    Category        NVARCHAR(100) NOT NULL,
                    ProficiencyLevel INT NOT NULL DEFAULT 80,
                    IconClass       NVARCHAR(100) NULL,
                    SortOrder       INT NOT NULL DEFAULT 0
                )");

            // Admin User (password: Admin@1234!)
            // BCrypt hash of "Admin@1234!" with work factor 12
            var existingAdmin = ExecuteScalar(conn,
                "SELECT COUNT(*) FROM AdminUsers WHERE Username = 'admin'");

            if (Convert.ToInt32(existingAdmin) == 0)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword("Admin@1234!", workFactor: 12);
                using var cmd = new SqlCommand(
                    "INSERT INTO AdminUsers (Username, PasswordHash) VALUES (@u, @h)", conn);
                cmd.Parameters.AddWithValue("@u", "admin");
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.ExecuteNonQuery();
            }

            // Projects
            var projCount = Convert.ToInt32(ExecuteScalar(conn, "SELECT COUNT(*) FROM Projects"));
            if (projCount == 0)
            {
                ExecuteNonQuery(conn, @"
                    INSERT INTO Projects (Title, Description, TechStack, ProjectUrl, GitHubUrl, IsFeatured) VALUES
                    ('E-Commerce Platform', 'Full-stack online shop with cart, payments and order tracking.', 'ASP.NET Core, MSSQL, Bootstrap', 'https://example.com', 'https://github.com', 1),
                    ('Task Manager App', 'Kanban-style task board with drag-and-drop and real-time updates.', 'React, Node.js, MongoDB', NULL, 'https://github.com', 1),
                    ('REST API Gateway', 'Centralised API gateway with rate limiting, auth and logging.', 'ASP.NET Core, Redis, Docker', NULL, 'https://github.com', 0)");
            }

            // Skills
            var skillCount = Convert.ToInt32(ExecuteScalar(conn, "SELECT COUNT(*) FROM Skills"));
            if (skillCount == 0)
            {
                ExecuteNonQuery(conn, @"
                    INSERT INTO Skills (Name, Category, ProficiencyLevel, SortOrder) VALUES
                    ('C#', 'Backend', 95, 1),
                    ('ASP.NET Core', 'Backend', 90, 2),
                    ('SQL Server', 'Backend', 85, 3),
                    ('JavaScript', 'Frontend', 88, 4),
                    ('HTML / CSS', 'Frontend', 92, 5),
                    ('Bootstrap', 'Frontend', 90, 6),
                    ('Git', 'Tools', 85, 7),
                    ('Docker', 'Tools', 70, 8)");
            }
        }

        private static void ExecuteNonQuery(SqlConnection conn, string sql)
        {
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private static object? ExecuteScalar(SqlConnection conn, string sql)
        {
            using var cmd = new SqlCommand(sql, conn);
            return cmd.ExecuteScalar();
        }
    }
}
