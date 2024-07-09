using AppAPI.Context;
using AppAPI.Models.Dto.TeacherDto;
using AppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using AppAPI.UtilityService;
using Microsoft.EntityFrameworkCore;
using AppAPI.Models.Dto.TeacherSubjectDto;
using System.Security.Claims;
using AppAPI.Models.Dto.TeacherClassDto;
using AppAPI.Models.Dto.EntityDto;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly ITeacherService _teacherService;
        private readonly ITeacherClassDisciplineService _classDisciplineService;

        public TeacherController(AppDbContext appDbContext, IConfiguration configuration, IJwtService jwtService, ITeacherService teacherService)
        {
            _context = appDbContext;
            _configuration = configuration;
            _jwtService = jwtService;
            _teacherService = teacherService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherDto teacherDto)
        {
            if (teacherDto == null)
                return BadRequest("Invalid data");

            try
            {
                string formattedFirstName = FormatName(teacherDto.FirstName);
                string formattedLastName = FormatName(teacherDto.LastName);
                string username = $"{teacherDto.FirstName.ToLower()}{teacherDto.LastName.ToLower()}";

                string password = GenerateRandomPassword();

                var user = new User
                {
                    FirstName = formattedFirstName,
                    LastName = formattedLastName,
                    Username = username,
                    Password = password,
                    Role = "Teacher",
                    CNP = teacherDto.CNP
                };

                await _context.Users.AddAsync(user);
                user.Token = _jwtService.CreateJwt(user, teacherDto.CNP);

                var teacher = new Teacher
                {
                    FirstName = formattedFirstName,
                    LastName = formattedLastName,
                    Username = username,
                    Password = password,
                    CNP = teacherDto.CNP,
                    TeacherSubjects = new List<TeacherSubject>(),
                    TeacherClasses = new List<TeacherClass>()
                };

                await _context.Teachers.AddAsync(teacher);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Username = teacher.Username,
                    Password = password
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
        private string FormatName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound("Teacher not found");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == teacher.Username);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                _context.Teachers.Remove(teacher);
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return Ok("Teacher and associated user deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("teacher-id")]
        public async Task<IActionResult> GetTeacherId(string firstName, string lastName)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.FirstName == firstName && t.LastName == lastName);

            if (teacher == null)
            {
                return NotFound();
            }

            return Ok(teacher.Id);
        }


        [HttpGet("current-teacher-id")]
        public async Task<IActionResult> GetCurrentTeacherId(string cnp)
        {
            try
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.CNP == cnp);

                if (teacher == null || teacher.Id == 0)
                {
                    return NotFound("Teacher ID not found");
                }

                return Ok(teacher.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("available-subjects")]
        public async Task<IActionResult> GetAvailableSubjectsForTeacher(int teacherId)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.TeacherSubjects)
                    .FirstOrDefaultAsync(t => t.Id == teacherId);

                if (teacher == null)
                {
                    return NotFound("Teacher not found");
                }

                var teacherSubjectIds = teacher.TeacherSubjects.Select(ts => ts.SubjectId);
                var availableSubjects = await _context.Subjects
                    .Where(s => !teacherSubjectIds.Contains(s.Id))
                    .ToListAsync();

                return Ok(availableSubjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("linked-subjects")]
        public async Task<IActionResult> GetLinkedSubjectsForTeacher(int teacherId)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                    .FirstOrDefaultAsync(t => t.Id == teacherId);

                if (teacher == null)
                {
                    return NotFound("Teacher not found");
                }

                var linkedSubjects = teacher.TeacherSubjects
                    .Select(ts => new GetSubjectDto
                    {
                        Id = ts.Subject.Id,
                        Name = ts.Subject.Title
                    })
                    .ToList();

                return Ok(linkedSubjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetTeachers()
        {
            try
            {
                var teachers = await _context.Teachers.ToListAsync();

                return Ok(teachers);            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{teacherId}/classes")]
        public async Task<ActionResult<List<Class>>> GetClassesForTeacher(int teacherId)
        {
            var classes = await _teacherService.GetClassesForTeacherAsync(teacherId);
            return Ok(classes);
        }

        [HttpGet("{teacherId}/subjects")]
        public async Task<ActionResult<List<Subject>>> GetSubjectsForTeacher(int teacherId)
        {
            var subjects = await _teacherService.GetSubjectsForTeacherAsync(teacherId);
            return Ok(subjects);
        }

        [HttpGet("{teacherId}/class/{classId}/subjects")]
        public async Task<ActionResult<List<Subject>>> GetSubjectsForTeacherAndClass(int teacherId, int classId)
        {
            var subjects = await _teacherService.GetSubjectsForTeacherAndClassAsync(teacherId, classId);
            return Ok(subjects);
        }


        [HttpGet("{teacherId}/class-discipline")]
        public async Task<ActionResult<List<TeacherClassDiscipline>>> GetClassDisciplinesForTeacher(int teacherId)
        {
            var classDisciplines = await _context.TeacherClassDisciplines
                .Where(tcd => tcd.TeacherId == teacherId)
                .Include(tcd => tcd.Class)
                .Include(tcd => tcd.Subject)
                .ToListAsync();

            return Ok(classDisciplines);
        }


        [HttpPost("add-teacher-class")]
        public async Task<IActionResult> AddTeacherClassLink([FromBody] AddTeacherClassDto teacherClassDto)
        {
            if (teacherClassDto == null)
                return BadRequest("Invalid data");

            try
            {
                var teacher = await _context.Teachers.FindAsync(teacherClassDto.TeacherId);
                var classEntity = await _context.Classes.FindAsync(teacherClassDto.ClassId);

                if (teacher == null)
                    return NotFound("Teacher not found");

                if (classEntity == null)
                    return NotFound("Class not found");

                var teacherClass = new TeacherClass
                {
                    TeacherId = teacherClassDto.TeacherId,
                    ClassId = teacherClassDto.ClassId
                };

                await _context.TeacherClasses.AddAsync(teacherClass);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Teacher-Class link added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("remove-teacher-class")]
        public async Task<IActionResult> DeleteTeacherClassLink(int teacherId, int classId)
        {
            try
            {
                var teacherClass = await _context.TeacherClasses
                    .FirstOrDefaultAsync(tc => tc.TeacherId == teacherId && tc.ClassId == classId);

                if (teacherClass == null)
                {
                    return NotFound("Teacher-Class link not found");
                }

                _context.TeacherClasses.Remove(teacherClass);
                await _context.SaveChangesAsync();

                return Ok("Teacher-Class link deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("linked-classes")]
        public async Task<IActionResult> GetLinkedClassesForTeacher(int teacherId)
        {
            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.TeacherClasses)
                    .ThenInclude(tc => tc.Class)
                    .FirstOrDefaultAsync(t => t.Id == teacherId);

                if (teacher == null)
                {
                    return NotFound("Teacher not found");
                }

                var linkedClasses = teacher.TeacherClasses
                    .Select(tc => new
                    {
                        tc.ClassId,
                        tc.Class.Series,
                        tc.Class.Number
                    })
                    .ToList();

                return Ok(linkedClasses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add-grade")]
        public async Task<IActionResult> AddGrade([FromBody] AddGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentSemester = await _context.Semesters
                .FirstOrDefaultAsync(s => s.StartDate <= gradeDto.Date && s.EndDate >= gradeDto.Date && !s.IsClosed);
            if (currentSemester == null)
            {
                return BadRequest("No active semester found for the given date.");
            }

            var existingGrade = await _context.Grades
                .AnyAsync(g => g.StudentId == gradeDto.StudentId && g.SubjectId == gradeDto.SubjectId && g.Date.Date == gradeDto.Date.Date);
            if (existingGrade)
            {
                return Conflict("A grade for this student in this subject on this date already exists.");
            }

            var newGrade = new Grade
            {
                StudentId = gradeDto.StudentId,
                SubjectId = gradeDto.SubjectId,
                Value = gradeDto.Value,
                Date = gradeDto.Date,
                SemesterId = currentSemester.Id
            };

            _context.Grades.Add(newGrade);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("update-grade")]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateGradeDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGrade = await _context.Grades
                .FirstOrDefaultAsync(g => g.StudentId == gradeDto.StudentId && g.SubjectId == gradeDto.SubjectId && g.Date.Date == gradeDto.Date.Date);

            if (existingGrade == null)
            {
                return NotFound("Nota nu a fost găsită.");
            }

            existingGrade.Value = gradeDto.Value;

            _context.Grades.Update(existingGrade);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-grade/{gradeId}")]
        public async Task<IActionResult> DeleteGrade(int gradeId)
        {
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null)
            {
                return NotFound();
            }

            try
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("student-subject-absences")]
        public async Task<IActionResult> GetStudentSubjectAbsences(int studentId, int subjectId, int month)
        {
            var absences = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && a.Date.Month == month && !a.IsPresent)
                .Select(a => a.Date)
                .ToListAsync();

            if (absences == null || !absences.Any())
            {
                return NotFound("No absences found for the specified student, subject, and month.");
            }

            return Ok(absences);
        }


        [HttpGet("available-classes-for-teacher")]
        public async Task<IActionResult> GetAvailableClassesForTeacher(int teacherId)
        {
            try
            {
                var linkedClassIds = await _context.TeacherClasses
                    .Where(tc => tc.TeacherId == teacherId)
                    .Select(tc => tc.ClassId)
                    .ToListAsync();

                var availableClasses = await _context.Classes
                    .Where(c => !linkedClassIds.Contains(c.ClassId))
                    .Select(c => new { c.ClassId, c.Series, c.Number })
                    .ToListAsync();

                return Ok(availableClasses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add-attendance")]
        public async Task<IActionResult> AddAttendance([FromBody] Attendance attendance)
        {
            var currentSemester = await _context.Semesters
                .FirstOrDefaultAsync(s => s.StartDate <= attendance.Date && s.EndDate >= attendance.Date && !s.IsClosed);
            if (currentSemester == null)
            {
                return BadRequest("No active semester found for the given date.");
            }

            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == attendance.StudentId && a.SubjectId == attendance.SubjectId && a.Date.Date == attendance.Date.Date);
            if (existingAttendance != null)
            {
                existingAttendance.IsPresent = attendance.IsPresent;
            }
            else
            {
                attendance.SemesterId = currentSemester.Id;
                _context.Attendances.Add(attendance);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("student/{studentId}/subject/{subjectId}/semester/{semesterId}/absences")]
        public async Task<IActionResult> GetAbsencesByStudentAndSemester(int studentId, int subjectId, int semesterId)
        {
            var absences = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && a.SemesterId == semesterId && !a.IsPresent)
                .Select(a => new AttendanceDto
                {
                    Date = a.Date,
                    IsPresent = a.IsPresent
                })
                .ToListAsync();

            if (absences == null || absences.Count == 0)
            {
                return NotFound("No absences found for the specified student, subject, and semester.");
            }

            return Ok(absences);
        }

        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTeachersWithDetails()
        {
            var teachers = await _context.Teachers
                .Select(t => new
                {
                    t.FirstName,
                    t.LastName,
                    Username = _context.Users.FirstOrDefault(u => u.CNP == t.CNP).Username,
                    Password = _context.Users.FirstOrDefault(u => u.CNP == t.CNP).Password
                })
                .ToListAsync();

            if (teachers == null || teachers.Count == 0)
            {
                return NotFound("No teachers found.");
            }

            return Ok(teachers);
        }


        [HttpGet("grades")]
        public async Task<IActionResult> GetGradesByStudentAndSubjectAndSemester(int studentId, int subjectId, int semesterId)
        {
            try
            {
                var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId);
                var semesterExists = await _context.Semesters.AnyAsync(s => s.Id == semesterId);

                if (!studentExists || !subjectExists || !semesterExists)
                {
                    return NotFound("Student, subject, or semester not found");
                }

                var grades = await _context.Grades
                    .Where(g => g.StudentId == studentId && g.SubjectId == subjectId && g.SemesterId == semesterId)
                    .ToListAsync();

                return Ok(grades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
