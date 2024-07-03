namespace AppAPI.Models
{
    public class EmailModel
    {
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailModel(string receiver, string subject, string content)
        {
            Receiver = receiver;
            Subject = subject;
            Content = content;
        }
    }
}
