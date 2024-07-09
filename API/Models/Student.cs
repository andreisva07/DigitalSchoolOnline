using System.Diagnostics;
using System.Text.Json.Serialization;

namespace AppAPI.Models
{
    public enum Gender
    {
        Male = 0,
        Female = 1,
        NonBinary =2
    }

    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string CNP { get; set; }
        public int ClassID { get; set; }

        [JsonIgnore]
        public Class Class { get; set; }
        public List<Grade> Grades { get; set; }
        public List<Attendance> Attendances { get; set; }
    }
}
