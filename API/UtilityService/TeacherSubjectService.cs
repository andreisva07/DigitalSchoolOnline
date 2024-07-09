using AppAPI.Context;
using AppAPI.Models;
using AppAPI.Models.Dto;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.UtilityService
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public TeacherSubjectService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetTeacherDto>> AddTeacherSubject(AddTeacherSubjectDto newTeacherSubject)

        {
            ServiceResponse<GetTeacherDto> response = new ServiceResponse<GetTeacherDto>();
            try
            {
                Teacher teacher = await _context.Teachers
                    .Include(x => x.TeacherSubjects).ThenInclude(x => x.Subject)
                    .FirstOrDefaultAsync(x => x.Id == newTeacherSubject.TeacherId);
                
                if(teacher == null)
                {
                    response.Success = false;
                    response.Message = "Teacher not found";
                    return response;
                }

                Subject subject = await _context.Subjects
                    .FirstOrDefaultAsync(x => x.Id == newTeacherSubject.SubjectId);
                
                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Subject not found";
                    return response;
                }

                TeacherSubject teacherSubject = new TeacherSubject
                {
                    Teacher = teacher,
                    Subject = subject
                };

                await _context.TeacherSubjects.AddAsync(teacherSubject);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetTeacherDto>(teacher);

            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<GetTeacherDto>> RemoveTeacherSubject(AddTeacherSubjectDto addTeacherSubjectDto)
    {
            var response = new ServiceResponse<GetTeacherDto>();

            try
            {
                Teacher teacher = await _context.Teachers
                    .Include(x => x.TeacherSubjects).ThenInclude(x => x.Subject)
                    .FirstOrDefaultAsync(x => x.Id == addTeacherSubjectDto.TeacherId);


                Subject subject = await _context.Subjects
                    .FirstOrDefaultAsync(x => x.Id == addTeacherSubjectDto.SubjectId);
                var teacherSubject = teacher.TeacherSubjects.FirstOrDefault(ts => ts.SubjectId == addTeacherSubjectDto.SubjectId);
                if (teacherSubject != null)
                {
                    _context.TeacherSubjects.Remove(teacherSubject);
                    await _context.SaveChangesAsync();
                    response.Message = "Teacher-subject association deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Teacher-subject association not found";
                }


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
