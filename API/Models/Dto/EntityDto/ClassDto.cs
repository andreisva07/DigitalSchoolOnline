namespace AppAPI.Models.Dto.EntityDto
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public int Number { get; set; }
        public string Series { get; set; }
        public List<StudentDto> Students { get; set; }
    }
}
