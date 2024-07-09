using AppAPI.Context;
using AppAPI.Models;
using AppAPI.Models.Dto.TeacherClassDto;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AppAPI.UtilityService
{
    public class TeacherClassDisciplineService : ITeacherClassDisciplineService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TeacherClassDisciplineService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
                _mapper = mapper;
        }

        public async Task<ServiceResponse<GetTeacherDto>> AddTeacherClassDiscipline(AddTeacherClassDisciplineDto addTeacherClassDisciplineDto)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            try
            {
                var teacher = await _context.Teachers
                    .Include(x => x.TeacherClasses)
                    .FirstOrDefaultAsync(x => x.Id == addTeacherClassDisciplineDto.TeacherId);

                if (teacher == null)
                {
                    response.Success = false;
                    response.Message = "Teacher not found";
                    return response;
                }

                var classEntity = await _context.Classes
                    .FirstOrDefaultAsync(x => x.ClassId == addTeacherClassDisciplineDto.ClassId);

                if (classEntity == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }

                var subject = await _context.Subjects
                    .FirstOrDefaultAsync(x => x.Id == addTeacherClassDisciplineDto.SubjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Subject not found";
                    return response;
                }

                var teacherClassDiscipline = new TeacherClassDiscipline
                {
                    Teacher = teacher,
                    Class = classEntity,
                    Subject = subject
                };

                _context.TeacherClassDisciplines.Add(teacherClassDiscipline);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetTeacherDto>(teacher);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
        public async Task<ServiceResponse<List<GetSubjectDto>>> GetAvailableSubjectsForTeacherClass(int teacherId, int classId)
        {
            var response = new ServiceResponse<List<GetSubjectDto>>();

            try
            {
                var linkedSubjects = await _context.TeacherSubjects
                    .Where(ts => ts.TeacherId == teacherId)
                    .Select(ts => ts.Subject)
                    .ToListAsync();

                var linkedClassSubjects = await _context.TeacherClassDisciplines
                    .Where(tcd => tcd.TeacherId == teacherId && tcd.ClassId == classId)
                    .Select(tcd => tcd.SubjectId)
                    .ToListAsync();

                var availableSubjects = linkedSubjects
                    .Where(subject => !linkedClassSubjects.Contains(subject.Id))
                    .Select(subject => new GetSubjectDto
                    {
                        Id = subject.Id,
                        Name = subject.Title
                    })
                    .ToList();

                response.Data = availableSubjects;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Internal server error: {ex.Message}";
            }

            return response;
        }
        public async Task<ServiceResponse<GetTeacherDto>> RemoveTeacherClassDiscipline(AddTeacherClassDisciplineDto removeTeacherClassDisciplineDto)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            try
            {
                var teacherClassDiscipline = await _context.TeacherClassDisciplines
                    .FirstOrDefaultAsync(x => x.TeacherId == removeTeacherClassDisciplineDto.TeacherId &&
                                              x.ClassId == removeTeacherClassDisciplineDto.ClassId &&
                                              x.SubjectId == removeTeacherClassDisciplineDto.SubjectId);

                if (teacherClassDiscipline != null)
                {
                    _context.TeacherClassDisciplines.Remove(teacherClassDiscipline);
                    await _context.SaveChangesAsync();
                    response.Message = "Teacher-class-discipline association deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Teacher-class-discipline association not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
        public async Task<ServiceResponse<bool>> CheckTeacherClassSubjectExists(int classId, int subjectId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var existingAssignment = await _context.TeacherClassDisciplines
                    .AnyAsync(tcd => tcd.ClassId == classId && tcd.SubjectId == subjectId);

                response.Data = existingAssignment;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

    }
}
