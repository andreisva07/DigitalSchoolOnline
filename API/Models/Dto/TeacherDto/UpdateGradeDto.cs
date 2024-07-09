namespace AppAPI.Models.Dto.TeacherDto
{
    public class UpdateGradeDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }

}
