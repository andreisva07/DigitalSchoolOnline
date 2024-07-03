using AppAPI.Models.Dto.EntityDto;

namespace AppAPI.Models.Dto.StudentDto
{
    public class StudentClassDto
    {
        public ClassDto Class { get; set; }
        public List<SubjectDto> Subjects { get; set; }
    }
}
