namespace AppAPI.Models
{
    public class StudentGradesViewModel
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Grade> Grades { get; set; }
    }
}
