using AppAPI.Context;
using AppAPI.Models;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public SubjectController(AppDbContext appDbContext, IConfiguration configuration, IMapper mapper)
        {
            _context = appDbContext;
            _configuration = configuration;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult<Subject>> GetAllSubjects()
        {
            return Ok(await _context.Subjects.ToListAsync());
        }

        [HttpGet("subject-by-id")]
        public async Task<GetSubjectDto> GetSubjectById(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                return null;
            }

            var subjectDto = _mapper.Map<GetSubjectDto>(subject);
            return subjectDto;
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> AddSubject([FromBody] GetSubjectDto addSubjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var subject = new Subject
                {
                    Title = addSubjectDto.Name,
                };
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();


                return CreatedAtAction(nameof(GetSubjectById), new { subjectId = subject.Id }, subject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("subject-id")]
        public async Task<IActionResult> GetSubjectId(string subjectName)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Title == subjectName);

            if (subject == null)
            {
                return NotFound();
            }

            return Ok(subject.Id);
        }

        [HttpGet("available-teachers")]
        public async Task<IActionResult> GetAvailableTeachersForSubject(int subjectId)
        {
            try
            {
                var subject = await _context.Subjects
                    .Include(s => s.TeacherSubjects)
                    .FirstOrDefaultAsync(s => s.Id == subjectId);

                if (subject == null)
                {
                    return NotFound("Subject not found");
                }

                var subjectTeacherIds = subject.TeacherSubjects.Select(ts => ts.TeacherId);
                var availableTeachers = await _context.Teachers
                    .Where(t => !subjectTeacherIds.Contains(t.Id))
                    .ToListAsync();

                return Ok(availableTeachers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}

