using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class PortfolioViewModel
    {
        public List<Project> Projects { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public List<string> SkillCategories { get; set; } = new();
    }
}
