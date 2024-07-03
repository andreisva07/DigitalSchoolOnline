using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherClassDto;
using AppAPI.UtilityService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherClassDisciplineController : ControllerBase
    {
        private readonly ITeacherClassDisciplineService _service;

        public TeacherClassDisciplineController(ITeacherClassDisciplineService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacherClassDiscipline(AddTeacherClassDisciplineDto dto)
        {
            var result = await _service.AddTeacherClassDiscipline(dto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveTeacherClassDiscipline(AddTeacherClassDisciplineDto removeTeacherClassDisciplineDto)
        {
            var result = await _service.RemoveTeacherClassDiscipline(removeTeacherClassDisciplineDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("available-subjects")]
        public async Task<IActionResult> GetAvailableSubjectsForTeacherClass(int teacherId, int classId)
        {
            var result = await _service.GetAvailableSubjectsForTeacherClass(teacherId, classId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
        [HttpGet("check-subject-assignment")]
        public async Task<IActionResult> CheckTeacherClassSubjectExists(int classId, int subjectId)
        {
            var result = await _service.CheckTeacherClassSubjectExists(classId, subjectId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

    }
}
