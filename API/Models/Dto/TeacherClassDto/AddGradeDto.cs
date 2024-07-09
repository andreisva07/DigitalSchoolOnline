using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Dto.TeacherClassDto
{
    public class AddGradeDto
    {
        public int StudentId { get; set; }

        public int SubjectId { get; set; }

        public int Value { get; set; }

        public DateTime Date { get; set; }
    }

}
