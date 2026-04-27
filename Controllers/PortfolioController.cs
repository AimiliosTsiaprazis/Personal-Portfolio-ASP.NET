using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Models;
using PortfolioApp.Repositories;

namespace PortfolioApp.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ISkillRepository _skillRepo;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(
            IProjectRepository projectRepo,
            ISkillRepository skillRepo,
            ILogger<PortfolioController> logger)
        {
            _projectRepo = projectRepo;
            _skillRepo = skillRepo;
            _logger = logger;
        }

        // One-Pager

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new PortfolioViewModel
            {
                Projects = (await _projectRepo.GetAllAsync()).ToList(),
                Skills = (await _skillRepo.GetAllAsync()).ToList()
            };
            vm.SkillCategories = vm.Skills.Select(s => s.Category).Distinct().OrderBy(c => c).ToList();
            return View(vm);
        }

        // CRUD

        [HttpGet("/api/projects")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectRepo.GetAllAsync();
            return Json(projects);
        }

        [HttpGet("/api/projects/{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Json(project);
        }

        [HttpPost("/api/projects")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _projectRepo.CreateAsync(project);
            _logger.LogInformation("Project created: {Title} (Id={Id})", project.Title, id);
            return Json(new { id, message = "Project created successfully." });
        }

        [HttpPut("/api/projects/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            project.Id = id;
            var success = await _projectRepo.UpdateAsync(project);
            if (!success) return NotFound();

            _logger.LogInformation("Project updated: Id={Id}", id);
            return Json(new { message = "Project updated successfully." });
        }

        [HttpDelete("/api/projects/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var success = await _projectRepo.DeleteAsync(id);
            if (!success) return NotFound();

            _logger.LogInformation("Project deleted: Id={Id}", id);
            return Json(new { message = "Project deleted." });
        }

        // Skills

        [HttpGet("/api/skills")]
        public async Task<IActionResult> GetSkills()
        {
            var skills = await _skillRepo.GetAllAsync();
            return Json(skills);
        }

        [HttpGet("/api/skills/{id}")]
        public async Task<IActionResult> GetSkill(int id)
        {
            var skill = await _skillRepo.GetByIdAsync(id);
            if (skill == null) return NotFound();
            return Json(skill);
        }

        [HttpPost("/api/skills")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSkill([FromBody] Skill skill)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _skillRepo.CreateAsync(skill);
            return Json(new { id, message = "Skill created successfully." });
        }

        [HttpPut("/api/skills/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] Skill skill)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            skill.Id = id;
            var success = await _skillRepo.UpdateAsync(skill);
            if (!success) return NotFound();

            return Json(new { message = "Skill updated." });
        }

        [HttpDelete("/api/skills/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var success = await _skillRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return Json(new { message = "Skill deleted." });
        }
    }
}
