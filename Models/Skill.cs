using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Skill name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Proficiency must be between 1 and 100.")]
        public int ProficiencyLevel { get; set; } = 80;

        [StringLength(100)]
        public string? IconClass { get; set; }

        public int SortOrder { get; set; } = 0;
    }
}
