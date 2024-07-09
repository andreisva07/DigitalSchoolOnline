using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherSubjectDto;

namespace AppAPI.UtilityService
{
    public interface ITeacherSubjectService
    {
        Task<ServiceResponse<GetTeacherDto>> AddTeacherSubject(AddTeacherSubjectDto newTeacherSubject);
        Task<ServiceResponse<GetTeacherDto>> RemoveTeacherSubject(AddTeacherSubjectDto addTeacherSubjectDto);
    }
}
