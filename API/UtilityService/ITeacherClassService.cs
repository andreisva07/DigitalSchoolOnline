using AppAPI.Models.Dto.TeacherSubjectDto;
using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherClassDto;

namespace AppAPI.UtilityService
{
    public interface ITeacherClassService
    {
        Task<ServiceResponse<GetTeacherDto>> AddTeacherClass(AddTeacherClassDto newTeacherClass);
        Task<ServiceResponse<GetTeacherDto>> RemoveTeacherClass(AddTeacherClassDto removeTeacherClass);
    }
}
