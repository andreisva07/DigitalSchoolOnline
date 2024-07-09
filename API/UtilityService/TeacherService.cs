using AppAPI.Context;
using AppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.UtilityService
{
    public class TeacherService: ITeacherService
    {
        private readonly AppDbContext _context;
        public TeacherService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Class>> GetClassesForTeacherAsync(int teacherId)
        {
            var classes = await _context.TeacherClasses
                .Where(tc => tc.TeacherId == teacherId)
                .Include(tc => tc.Class)
                    .ThenInclude(c => c.Students)
                .Include(tc => tc.Teacher)
                .Select(tc => tc.Class)
                .ToListAsync();

            return classes;
        }

        public async Task<List<Subject>> GetSubjectsForTeacherAsync(int teacherId)
        {
            var subjects = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .Include(ts => ts.Subject)
                .Select(ts => ts.Subject)
                .ToListAsync();

            return subjects;
        }
        public async Task<List<Subject>> GetSubjectsForTeacherAndClassAsync(int teacherId, int classId)
        {
            var subjects = await _context.TeacherClassDisciplines
                .Where(tcd => tcd.TeacherId == teacherId && tcd.ClassId == classId)
                .Select(tcd => tcd.Subject)
                .ToListAsync();

            return subjects;
        }
    }
}
