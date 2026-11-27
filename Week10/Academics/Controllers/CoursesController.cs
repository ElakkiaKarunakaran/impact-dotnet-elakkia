using Academics.Data;
using Academics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academics.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly AcademicsContext _db;
    public CoursesController(AcademicsContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Courses.AsNoTracking().ToListAsync());

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Add([FromBody] Course course)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return Created($"/api/courses/{course.Id}", course);
    }
}
