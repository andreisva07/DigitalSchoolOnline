using AppAPI.Models;

namespace AppAPI.UtilityService
{
    public interface ITeacherService
    {
        Task<List<Class>> GetClassesForTeacherAsync(int teacherId);
        Task<List<Subject>> GetSubjectsForTeacherAsync(int teacherId);
        Task<List<Subject>> GetSubjectsForTeacherAndClassAsync(int teacherId, int classId);

    }
}
