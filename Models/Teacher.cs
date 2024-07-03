namespace AppAPI.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CNP { get; set; }
        public List<TeacherSubject> TeacherSubjects{ get; set; }
        public List<TeacherClass> TeacherClasses { get; set; }

    }
}
