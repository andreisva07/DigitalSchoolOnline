using AppAPI.Context;
using AppAPI.Models;
using AppAPI.Models.Dto.EntityDto;
using AppAPI.Models.Dto.NewFolder;
using AppAPI.Models.Dto.StudentDto;
using AppAPI.UtilityService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public StudentController(AppDbContext appDbContext, IConfiguration configuration, IMapper mapper, IJwtService jwtService)
        {
            _context = appDbContext;
            _configuration = configuration;
            _mapper = mapper;
            _jwtService = jwtService;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPost]
        public async Task<ActionResult<AddStudentDto>> AddStudent(AddStudentDto studentDto)
        {
            try
            {
                string formattedFirstName = FormatName(studentDto.FirstName);
                string formattedLastName = FormatName(studentDto.LastName);

                var student = new Student
                {
                    FirstName = formattedFirstName,
                    LastName = formattedLastName,
                    Gender = studentDto.Gender,
                    ClassID = studentDto.ClassId,
                    CNP = studentDto.CNP
                };

                _context.Students.Add(student);

                var user = new User
                {
                    FirstName = formattedFirstName,
                    LastName = formattedLastName,
                    Username = $"{student.FirstName.ToLower()}{student.LastName.ToLower()}",
                    Password = GenerateRandomPassword(),
                    Role = "Student",
                    CNP = student.CNP
                };

                user.Token = _jwtService.CreateJwt(user, studentDto.CNP);

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                var responseDto = new AddStudentDto
                {
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Gender = student.Gender,
                    ClassId = student.ClassID,
                    CNP = student.CNP
                };

                return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("class/{classId}/grades")]
        public async Task<ActionResult<IEnumerable<StudentGradesViewModel>>> GetStudentsWithGradesByClass(int classId)
        {
            var studentsWithGrades = await _context.Students
                .Include(s => s.Grades) // Includeți notele elevilor în interogare
                .Where(s => s.ClassID == classId)
                .Select(s => new StudentGradesViewModel
                {
                    StudentId = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Grades = s.Grades.ToList() // Lista notelor elevului
                })
                .ToListAsync();

            if (studentsWithGrades == null || studentsWithGrades.Count == 0)
            {
                return NotFound("No students found for the given class ID.");
            }

            return studentsWithGrades;
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentClass(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        [HttpGet("class/{classId}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByClassId(int classId)
        {
            var students = await _context.Students.Where(s => s.ClassID == classId).ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found for the given class ID.");
            }

            return students;
        }

        [HttpGet("class/{classId}/subject/{subjectId}/grades")]
        public async Task<ActionResult<IEnumerable<StudentGradesViewModel>>> GetStudentsWithGradesByClassAndSubject(int classId, int subjectId)
        {
            var studentsWithGrades = await _context.Students
                .Include(s => s.Grades)
                .Where(s => s.ClassID == classId)
                .Select(s => new StudentGradesViewModel
                {
                    StudentId = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Grades = s.Grades.Where(g => g.SubjectId == subjectId).ToList()
                })
                .ToListAsync();

            if (studentsWithGrades == null || studentsWithGrades.Count == 0)
            {
                return NotFound("No students found for the given class and subject.");
            }

            return studentsWithGrades;
        }

        [HttpGet("student-id")]
        public async Task<IActionResult> GetStudentId(string cnp)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.CNP == cnp);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(student.Id);
        }

        [HttpGet("{studentId}/class-and-subjects")]
        public async Task<ActionResult<StudentClassDto>> GetStudentClassAndSubjects(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .ThenInclude(c => c.TeacherClasses)
                .ThenInclude(tc => tc.Teacher)
                .ThenInclude(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var subjects = student.Class.TeacherClasses
                .SelectMany(tc => tc.Teacher.TeacherSubjects)
                .Select(ts => ts.Subject)
                .Distinct()
                .ToList();

            var studentClassDto = new StudentClassDto
            {
                Class = new ClassDto
                {
                    Series = student.Class.Series,
                    Number = student.Class.Number
                },
                Subjects = subjects.Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Title = s.Title
                }).ToList()
            };

            return studentClassDto;
        }


        [HttpGet("{studentId}/class-subjects")]
        public async Task<IActionResult> GetStudentClassAndSubjectsForStudent(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var subjects = await _context.TeacherClassDisciplines
                .Where(tcd => tcd.ClassId == student.ClassID)
                .Include(tcd => tcd.Subject)
                .Select(tcd => new
                {
                    tcd.Subject.Id,
                    tcd.Subject.Title
                })
                .ToListAsync();

            return Ok(new { student.Class, Subjects = subjects });
        }


        [HttpGet("{studentId}/subject/{subjectId}/grades")]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetGradesByStudentAndSubject(int studentId, int subjectId)
        {
            var grades = await _context.Grades
                .Where(g => g.StudentId == studentId && g.SubjectId == subjectId)
                .Select(g => new GradeDto
                {
                    Value = g.Value,
                    Date = g.Date
                })
                .ToListAsync();

            return grades;
        }

        [HttpGet("{studentId}/subject/{subjectId}/attendance")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendanceByStudentAndSubject(int studentId, int subjectId)
        {
            var attendanceRecords = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId)
                .Select(a => new AttendanceDto
                {
                    Date = a.Date,
                    IsPresent = a.IsPresent
                })
                .ToListAsync();

            return attendanceRecords;
        }

        [HttpGet("student-subject-absences")]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAbsencesByStudentSubjectAndMonth(int studentId, int subjectId, int month)
        {
            var year = DateTime.Now.Year;
            var absences = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && a.Date.Month == month && a.Date.Year == year && !a.IsPresent)
                .Select(a => a.Date)
                .ToListAsync();

            return Ok(absences);
        }

        [HttpPut("transfer")]
        public async Task<IActionResult> TransferStudent([FromBody] TransferStudentDto transferDto)
        {
            var student = await _context.Students.FindAsync(transferDto.StudentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            student.ClassID = transferDto.NewClassId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student transferred successfully" });
        }

        [HttpGet("class/{classId}/details")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentsByClassWithDetails(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassID == classId)
                .Select(s => new
                {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    Series = s.Class.Series,
                    Number = s.Class.Number,
                    Username = _context.Users.FirstOrDefault(u => u.CNP == s.CNP).Username,
                    Password = _context.Users.FirstOrDefault(u => u.CNP == s.CNP).Password
                })
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found for the given class ID.");
            }

            return Ok(students);
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest(new { message = "Student ID mismatch." });
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound(new { message = "Student not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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

    }
}
