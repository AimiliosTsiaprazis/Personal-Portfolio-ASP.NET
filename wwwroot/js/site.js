/* ============================================================
   Portfolio – site.js
   Full CRUD via JSON API endpoints
   ============================================================ */

'use strict';

// ── Helpers ──────────────────────────────────────────────────────────────────

function getCsrfToken() {
    // Pull from the hidden meta or the inline script variable
    if (typeof CSRF_TOKEN !== 'undefined' && CSRF_TOKEN) return CSRF_TOKEN;
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.getAttribute('content') : '';
}

async function apiFetch(url, method, body) {
    const opts = {
        method,
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': getCsrfToken(),
            'RequestVerificationToken': getCsrfToken()
        }
    };
    if (body) opts.body = JSON.stringify(body);
    const res = await fetch(url, opts);
    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || `HTTP ${res.status}`);
    }
    return res.json();
}

function showToast(message, type = 'success') {
    const toast = document.getElementById('toastNotification');
    const msg   = document.getElementById('toastMessage');
    if (!toast || !msg) return;
    msg.textContent = message;
    toast.className = `toast align-items-center border-0 text-white bg-${type === 'success' ? 'success' : 'danger'}`;
    const bsToast = bootstrap.Toast.getOrCreateInstance(toast, { delay: 3000 });
    bsToast.show();
}

// ── Navbar shrink on scroll ───────────────────────────────────────────────────
window.addEventListener('scroll', () => {
    const nav = document.getElementById('mainNav');
    if (nav) nav.classList.toggle('py-1', window.scrollY > 50);
});

// ── Skill range display ───────────────────────────────────────────────────────
const skillLevelInput = document.getElementById('skillLevel');
const skillLevelDisplay = document.getElementById('skillLevelDisplay');
if (skillLevelInput && skillLevelDisplay) {
    skillLevelInput.addEventListener('input', () => {
        skillLevelDisplay.textContent = skillLevelInput.value;
    });
}

// ════════════════════════════════════════════════════════════════════════════
//  PROJECTS
// ════════════════════════════════════════════════════════════════════════════

const projectModal = document.getElementById('addProjectModal');
const projectModalLabel = document.getElementById('projectModalTitle');
const saveProjectBtn = document.getElementById('saveProjectBtn');

// Reset modal to "Add" state
function resetProjectModal() {
    document.getElementById('projectId').value = '';
    document.getElementById('projectTitle').value = '';
    document.getElementById('projectDescription').value = '';
    document.getElementById('projectTechStack').value = '';
    document.getElementById('projectUrl').value = '';
    document.getElementById('projectGitHubUrl').value = '';
    document.getElementById('projectImageUrl').value = '';
    document.getElementById('projectIsFeatured').checked = false;
    if (projectModalLabel) projectModalLabel.textContent = 'Add Project';
}

// When modal opens via navbar "Add Project" link → always reset
if (projectModal) {
    projectModal.addEventListener('show.bs.modal', () => {
        if (!document.getElementById('projectId').value) resetProjectModal();
    });
    projectModal.addEventListener('hidden.bs.modal', resetProjectModal);
}

// Edit buttons – populate modal
document.addEventListener('click', (e) => {
    const btn = e.target.closest('.btn-edit-project');
    if (!btn) return;

    document.getElementById('projectId').value = btn.dataset.id;
    document.getElementById('projectTitle').value = btn.dataset.title || '';
    document.getElementById('projectDescription').value = btn.dataset.description || '';
    document.getElementById('projectTechStack').value = btn.dataset.techstack || '';
    document.getElementById('projectUrl').value = btn.dataset.projecturl || '';
    document.getElementById('projectGitHubUrl').value = btn.dataset.githuburl || '';
    document.getElementById('projectImageUrl').value = btn.dataset.imageurl || '';
    document.getElementById('projectIsFeatured').checked = btn.dataset.isfeatured === 'true';
    if (projectModalLabel) projectModalLabel.textContent = 'Edit Project';

    bootstrap.Modal.getOrCreateInstance(document.getElementById('addProjectModal')).show();
});

// Save project (create or update)
if (saveProjectBtn) {
    saveProjectBtn.addEventListener('click', async () => {
        const id    = document.getElementById('projectId').value;
        const title = document.getElementById('projectTitle').value.trim();
        const desc  = document.getElementById('projectDescription').value.trim();

        if (!title || !desc) {
            showToast('Title and Description are required.', 'error');
            return;
        }

        const payload = {
            title,
            description: desc,
            techStack:   document.getElementById('projectTechStack').value.trim() || null,
            projectUrl:  document.getElementById('projectUrl').value.trim() || null,
            gitHubUrl:   document.getElementById('projectGitHubUrl').value.trim() || null,
            imageUrl:    document.getElementById('projectImageUrl').value.trim() || null,
            isFeatured:  document.getElementById('projectIsFeatured').checked
        };

        try {
            saveProjectBtn.disabled = true;
            saveProjectBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Saving...';

            if (id) {
                await apiFetch(`/api/projects/${id}`, 'PUT', payload);
                showToast('Project updated successfully!');
            } else {
                await apiFetch('/api/projects', 'POST', payload);
                showToast('Project created successfully!');
            }
            bootstrap.Modal.getInstance(document.getElementById('addProjectModal')).hide();
            setTimeout(() => location.reload(), 800);
        } catch (err) {
            showToast('Error saving project: ' + err.message, 'error');
        } finally {
            saveProjectBtn.disabled = false;
            saveProjectBtn.innerHTML = '<i class="bi bi-save me-2"></i>Save Project';
        }
    });
}

// ════════════════════════════════════════════════════════════════════════════
//  SKILLS
// ════════════════════════════════════════════════════════════════════════════

const skillModal = document.getElementById('addSkillModal');
const skillModalLabel = document.getElementById('skillModalTitle');
const saveSkillBtn = document.getElementById('saveSkillBtn');

function resetSkillModal() {
    document.getElementById('skillId').value = '';
    document.getElementById('skillName').value = '';
    document.getElementById('skillCategory').value = '';
    document.getElementById('skillLevel').value = 80;
    if (skillLevelDisplay) skillLevelDisplay.textContent = '80';
    document.getElementById('skillSortOrder').value = 0;
    if (skillModalLabel) skillModalLabel.textContent = 'Add Skill';
}

if (skillModal) {
    skillModal.addEventListener('show.bs.modal', () => {
        if (!document.getElementById('skillId').value) resetSkillModal();
    });
    skillModal.addEventListener('hidden.bs.modal', resetSkillModal);
}

// Edit skill buttons
document.addEventListener('click', (e) => {
    const btn = e.target.closest('.btn-edit-skill');
    if (!btn) return;

    document.getElementById('skillId').value = btn.dataset.id;
    document.getElementById('skillName').value = btn.dataset.name || '';
    document.getElementById('skillCategory').value = btn.dataset.category || '';
    document.getElementById('skillLevel').value = btn.dataset.level || 80;
    if (skillLevelDisplay) skillLevelDisplay.textContent = btn.dataset.level || 80;
    document.getElementById('skillSortOrder').value = btn.dataset.sortorder || 0;
    if (skillModalLabel) skillModalLabel.textContent = 'Edit Skill';

    bootstrap.Modal.getOrCreateInstance(document.getElementById('addSkillModal')).show();
});

// Save skill
if (saveSkillBtn) {
    saveSkillBtn.addEventListener('click', async () => {
        const id   = document.getElementById('skillId').value;
        const name = document.getElementById('skillName').value.trim();
        const cat  = document.getElementById('skillCategory').value.trim();

        if (!name || !cat) {
            showToast('Skill name and category are required.', 'error');
            return;
        }

        const payload = {
            name,
            category:        cat,
            proficiencyLevel: parseInt(document.getElementById('skillLevel').value, 10),
            sortOrder:        parseInt(document.getElementById('skillSortOrder').value, 10) || 0
        };

        try {
            saveSkillBtn.disabled = true;
            saveSkillBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Saving...';

            if (id) {
                await apiFetch(`/api/skills/${id}`, 'PUT', payload);
                showToast('Skill updated successfully!');
            } else {
                await apiFetch('/api/skills', 'POST', payload);
                showToast('Skill created successfully!');
            }
            bootstrap.Modal.getInstance(document.getElementById('addSkillModal')).hide();
            setTimeout(() => location.reload(), 800);
        } catch (err) {
            showToast('Error saving skill: ' + err.message, 'error');
        } finally {
            saveSkillBtn.disabled = false;
            saveSkillBtn.innerHTML = '<i class="bi bi-save me-2"></i>Save Skill';
        }
    });
}

// ════════════════════════════════════════════════════════════════════════════
//  DELETE (shared confirm modal)
// ════════════════════════════════════════════════════════════════════════════

let deleteUrl = '';

document.addEventListener('click', (e) => {
    // Project delete
    const projBtn = e.target.closest('.btn-delete-project');
    if (projBtn) {
        deleteUrl = `/api/projects/${projBtn.dataset.id}`;
        document.getElementById('deleteTargetName').textContent = projBtn.dataset.title || 'this project';
        bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmDeleteModal')).show();
        return;
    }

    // Skill delete
    const skillBtn = e.target.closest('.btn-delete-skill');
    if (skillBtn) {
        deleteUrl = `/api/skills/${skillBtn.dataset.id}`;
        document.getElementById('deleteTargetName').textContent = skillBtn.dataset.name || 'this skill';
        bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmDeleteModal')).show();
    }
});

const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
if (confirmDeleteBtn) {
    confirmDeleteBtn.addEventListener('click', async () => {
        if (!deleteUrl) return;
        try {
            confirmDeleteBtn.disabled = true;
            await apiFetch(deleteUrl, 'DELETE');
            showToast('Deleted successfully.');
            bootstrap.Modal.getInstance(document.getElementById('confirmDeleteModal')).hide();
            setTimeout(() => location.reload(), 800);
        } catch (err) {
            showToast('Error deleting: ' + err.message, 'error');
        } finally {
            confirmDeleteBtn.disabled = false;
            deleteUrl = '';
        }
    });
}
