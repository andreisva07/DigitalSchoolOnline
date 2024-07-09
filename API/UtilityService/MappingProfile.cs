using AppAPI.Models;
using AutoMapper;
using AppAPI.Models.Dto.TeacherSubjectDto;
using AppAPI.Models.Dto.TeacherClassDto;

namespace AppAPI.UtilityService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Teacher, GetTeacherDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TeacherSubjects.Select(x => x.Subject)));

            CreateMap<Subject, GetSubjectDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));


            CreateMap<Class, GetClassDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClassId))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series));
        }
    }
}
