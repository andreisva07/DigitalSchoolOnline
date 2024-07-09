using AppAPI.Models.Dto.TeacherClassDto;
using AppAPI.Models.Dto.TeacherSubjectDto;

namespace AppAPI.UtilityService
{
    public interface ITeacherClassDisciplineService
    {
        Task<ServiceResponse<GetTeacherDto>> AddTeacherClassDiscipline(AddTeacherClassDisciplineDto addTeacherClassDisciplineDto);
        Task<ServiceResponse<GetTeacherDto>> RemoveTeacherClassDiscipline(AddTeacherClassDisciplineDto removeTeacherClassDisciplineDto);
        Task<ServiceResponse<List<GetSubjectDto>>> GetAvailableSubjectsForTeacherClass(int teacherId, int classId);
        Task<ServiceResponse<bool>> CheckTeacherClassSubjectExists(int classId, int subjectId);


    }
}
