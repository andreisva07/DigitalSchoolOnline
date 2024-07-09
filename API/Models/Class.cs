namespace AppAPI.Models
{
    public class Class
    {
        public int ClassId { get; set; }
        public int Number { get; set; }
        public string Series {  get; set; }
        public List<Student> Students{ get; set;}
        public List<TeacherClass> TeacherClasses { get; set;}


    }
}
