using AppAPI.Models;
using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherClassDto;
using AppAPI.UtilityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherClassController : ControllerBase
    {
        private readonly ITeacherClassService _teacherClassService;
        public TeacherClassController(ITeacherClassService teacherClassService)
        {
            _teacherClassService = teacherClassService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacherSubject(AddTeacherClassDto addTeacherClassDto)
        {
            return Ok(await _teacherClassService.AddTeacherClass(addTeacherClassDto));
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveTeacherSubject([FromQuery] int teacherId, [FromQuery] int classId)
        {
            var addTeacherClassDto = new AddTeacherClassDto
            {
                TeacherId = teacherId,
                ClassId = classId
            };
            var result = await _teacherClassService.RemoveTeacherClass(addTeacherClassDto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
