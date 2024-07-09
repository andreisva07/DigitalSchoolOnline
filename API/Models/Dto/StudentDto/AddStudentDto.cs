    namespace AppAPI.Models.Dto.NewFolder
    {
        public class AddStudentDto
        {
         
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Gender Gender { get; set; }
            public int ClassId { get; set; }
            public string CNP { get; set; }
        }
    }

