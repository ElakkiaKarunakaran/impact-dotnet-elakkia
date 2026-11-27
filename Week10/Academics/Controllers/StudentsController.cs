using Academics.Data;
using Academics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academics.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]  // Task 10.3 — any authenticated user can read.
public class StudentsController : ControllerBase
{
    private readonly AcademicsContext _db;
    public StudentsController(AcademicsContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Students.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Students.FindAsync(id);
        return s is null ? NotFound() : Ok(s);
    }

    // Task 10.3 — Teacher-only writes (gateway + service both enforce).
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Add([FromBody] Student student)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _db.Students.AnyAsync(s => s.RollNumber == student.RollNumber))
            return BadRequest($"Roll number {student.RollNumber} already exists.");

        student.Id = 0;
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(int id, [FromBody] Student input)
    {
        var existing = await _db.Students.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name       = input.Name;
        existing.Age        = input.Age;
        existing.RollNumber = input.RollNumber;
        existing.Email      = input.Email;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Students.FindAsync(id);
        if (existing is null) return NotFound();
        _db.Students.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("~/api/academics/health")]
    [AllowAnonymous]
    public IActionResult Health() => Ok(new { service = "academics", status = "ok" });
}
