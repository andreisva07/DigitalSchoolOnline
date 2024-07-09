namespace AppAPI.Models.Dto.TeacherSubjectDto
{
    public class GetTeacherDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<GetSubjectDto> Subjects { get; set; }
    }
}
