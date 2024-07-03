using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AppAPI.UtilityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSubjectController : ControllerBase

    {
        private readonly ITeacherSubjectService _teacherSubjectService;
        public TeacherSubjectController(ITeacherSubjectService teacherSubjectService)
        {
            _teacherSubjectService = teacherSubjectService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacherSubject(AddTeacherSubjectDto addTeacherSubjectDto)
        {
            return Ok(await _teacherSubjectService.AddTeacherSubject(addTeacherSubjectDto));
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveTeacherSubject([FromQuery] int teacherId, [FromQuery] int subjectId)
        {
            var addTeacherSubjectDto = new AddTeacherSubjectDto
            {
                TeacherId = teacherId,
                SubjectId = subjectId
            };

            var result = await _teacherSubjectService.RemoveTeacherSubject(addTeacherSubjectDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
