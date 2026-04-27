using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(300)]
        public string? TechStack { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(500)]
        public string? ProjectUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(500)]
        public string? GitHubUrl { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
