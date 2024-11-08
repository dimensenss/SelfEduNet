namespace SelfEduNet.Configurations
{
    public class EmailSMTPSettings
    {
        public string SmtpServer { get; set; } = default!;
        public int Port { get; set; } = default!;
        public string SenderEmail { get; set; } = default!;
        public string SenderPassword { get; set; } = default!;

    }
}
