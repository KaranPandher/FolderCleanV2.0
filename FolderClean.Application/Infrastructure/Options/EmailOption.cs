namespace FolderClean.Application.Infrastructure.Options
{
    public class EmailOption
    {
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string TargetEmail { get; set; }
    }
}