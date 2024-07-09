using AppAPI.Context;
using AppAPI.Models;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SemesterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddSemester([FromBody] Semester semester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSemesterById), new { id = semester.Id}, semester);
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);

            if (semester == null)
            {
                return NotFound();
            }

            return Ok(semester);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllSemesters()
        {
            var semesters = await _context.Semesters.ToListAsync();

            if (semesters == null || semesters.Count == 0)
            {
                return NotFound("No semesters found.");
            }

            return Ok(semesters);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSemester()
        {
            var currentSemester = await _context.Semesters
                .Where(s => !s.IsClosed)
                .OrderBy(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (currentSemester == null)
            {
                return NotFound("No active semester found.");
            }

            return Ok(currentSemester);
        }


        [HttpGet("closed")]
        public async Task<IActionResult> GetClosedSemesters()
        {
            var closedSemesters = await _context.Semesters
                .Where(s => s.IsClosed)
                .ToListAsync();

            if (closedSemesters == null || !closedSemesters.Any())
            {
                return NotFound("No closed semesters found.");
            }

            return Ok(closedSemesters);
        }

        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> ReactivateSemester(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);

            if (semester == null)
            {
                return NotFound();
            }

            semester.IsClosed = false;
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();

            return Ok(semester);
        }

        [HttpDelete("{id}/remove")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            try
            {
                var semester = await _context.Semesters.FindAsync(id);
                if (semester == null)
                {
                    return NotFound();
                }

                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseSemester(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);

            if (semester == null)
            {
                return NotFound();
            }

            semester.IsClosed = true;
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();

            return Ok(semester);
        }
    }
}
