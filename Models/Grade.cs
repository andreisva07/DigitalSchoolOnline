namespace AppAPI.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }
        public int? SemesterId { get; set; }
        public Semester? Semester { get; set; }
    }
}
