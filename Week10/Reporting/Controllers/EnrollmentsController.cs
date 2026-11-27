using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reporting.Data;

namespace Reporting.Controllers;

// Task 10.4 — read-mostly endpoints over the pre-existing reporting schema.
[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly ReportingContext _db;
    public EnrollmentsController(ReportingContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Enrollments.AsNoTracking().OrderBy(e => e.CourseCode).ToListAsync());

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
        => Ok(await _db.EnrollmentSummary.AsNoTracking().ToListAsync());

    [HttpGet("by-course/{code}")]
    public async Task<IActionResult> ByCourse(string code)
        => Ok(await _db.Enrollments.AsNoTracking()
                     .Where(e => e.CourseCode == code)
                     .ToListAsync());

    [HttpGet("~/api/reporting/health")]
    [AllowAnonymous]
    public IActionResult Health() => Ok(new { service = "reporting", status = "ok" });
}
