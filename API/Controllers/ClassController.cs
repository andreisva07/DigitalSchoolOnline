using AppAPI.Context;
using AppAPI.Helpers;
using AppAPI.Models;
using AppAPI.Models.Dto.TeacherClassDto;
using AutoMapper;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public ClassController(AppDbContext appDbContext, IConfiguration configuration, IMapper mapper)
        {
            _context = appDbContext;
            _configuration = configuration;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult<Class>> GetAllClassrooms()
        {
            return Ok(await _context.Classes.ToListAsync());
        }


        [HttpGet("class-by-id")]
        public async Task<GetClassDto> GetClassById(int classId)
        {
            var subject = await _context.Classes.FindAsync(classId);
            if (subject == null)
            {
                return null;
            }

            var classDto = _mapper.Map<GetClassDto>(subject);
            return classDto;
        }
        [HttpGet("class-id")]
        public async Task<IActionResult> GetClassId(string series, int number)
        {
            var schoolClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.Series == series && c.Number == number);

            if (schoolClass == null)
            {
                return NotFound(); 
            }

            return Ok(schoolClass.ClassId);
        }


        [HttpPost]
        public async Task<ActionResult<Class>> AddClass([FromBody] GetClassDto classDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!IsValidClassNumber(classDto.Number) || !IsValidClassSeries(classDto.Series))
                {
                    return BadRequest("Invalid class number or series.");
                }

                var existingClass = await _context.Classes.FirstOrDefaultAsync(c => c.Series.ToUpper() == classDto.Series.ToUpper() && c.Number == classDto.Number);
                if (existingClass != null)
                {
                    return Conflict("Class already exists.");
                }

                var newClass = new Class
                {
                    Series = classDto.Series.ToUpper(),
                    Number = classDto.Number
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClassById), new { classId = newClass.ClassId }, newClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsValidClassNumber(int number)
        {
            return number >= 0 && number <= 12;
        }

        private bool IsValidClassSeries(string series)
        {
            return series.Length <= 2 && series.All(c => c >= 'a' && c <= 'f');
        }


        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Class>>> GetAvailableClasses()
        {
            var classes = await _context.Classes.ToListAsync();
            return classes;
        }

        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<Student>>> GetClassStudents(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassID == classId)
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found for this class");
            }

            return Ok(students);
        }

        [HttpGet("attendance-by-date")]
        public async Task<IActionResult> GetAttendanceByClassAndDate(int classId, int subjectId, DateTime date)
        {
            var attendanceRecords = await _context.Attendances
                .Where(a => a.Student.ClassID == classId && a.SubjectId == subjectId && a.Date.Date == date.Date)
                .ToListAsync();

            if (attendanceRecords == null || attendanceRecords.Count == 0)
            {
                return NotFound("No attendance records found for the given class, subject, and date.");
            }

            return Ok(attendanceRecords);
        }

        [HttpGet("student/{studentId}/subject/{subjectId}/grades")]
        public async Task<IActionResult> GetGradesByStudentAndSubject(int studentId, int subjectId)
        {
            var grades = await _context.Grades
                .Where(g => g.StudentId == studentId && g.SubjectId == subjectId)
                .Select(g => new { g.Value, g.Date })
                .ToListAsync();

            return Ok(grades);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var subject = await _context.Classes.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            try
            {
                _context.Classes.Remove(subject);
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
