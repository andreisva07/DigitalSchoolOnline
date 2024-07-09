using AppAPI.Context;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AppAPI.Models.Dto;
using AppAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AppAPI.Models.Dto.TeacherClassDto;

namespace AppAPI.UtilityService
{
    public class TeacherClassService : ITeacherClassService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public TeacherClassService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetTeacherDto>> AddTeacherClass(AddTeacherClassDto newTeacherClass)

        {
            ServiceResponse<GetTeacherDto> response = new ServiceResponse<GetTeacherDto>();
            try
            {
                Teacher teacher = await _context.Teachers
                    .Include(x => x.TeacherClasses).ThenInclude(x => x.Class)
                    .FirstOrDefaultAsync(x => x.Id == newTeacherClass.TeacherId);

                if (teacher == null)
                {
                    response.Success = false;
                    response.Message = "Teacher not found";
                    return response;
                }

                Class subject = await _context.Classes
                    .FirstOrDefaultAsync(x => x.ClassId == newTeacherClass.ClassId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }

                TeacherClass teacherClass = new TeacherClass
                {
                    Teacher = teacher,
                    Class = subject
                };

                await _context.TeacherClasses.AddAsync(teacherClass);
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
        public async Task<ServiceResponse<GetTeacherDto>> RemoveTeacherClass(AddTeacherClassDto removeTeacherClass)
        {
            var response = new ServiceResponse<GetTeacherDto>();

            try
            {
                Teacher teacher = await _context.Teachers
                    .Include(x => x.TeacherClasses).ThenInclude(x => x.Class)
                    .FirstOrDefaultAsync(x => x.Id == removeTeacherClass.TeacherId);


                Class subject = await _context.Classes
                    .FirstOrDefaultAsync(x => x.ClassId == removeTeacherClass.ClassId);
                var teacherClass = teacher.TeacherClasses.FirstOrDefault(ts => ts.ClassId == removeTeacherClass.ClassId);
                if (teacherClass != null)
                {
                    _context.TeacherClasses.Remove(teacherClass);
                    await _context.SaveChangesAsync();
                    response.Message = "Teacher-class association deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Teacher-class association not found";
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
