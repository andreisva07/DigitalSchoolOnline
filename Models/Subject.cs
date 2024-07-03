using System.Diagnostics;

namespace AppAPI.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<TeacherSubject> TeacherSubjects { get; set; }
        public List<Grade> Grades { get; set; }
        public List<Attendance> Attendances { get; set; }

    }
}
