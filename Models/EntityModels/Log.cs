namespace Mobile.Models.EntityModels
{
    public class Log
    {
        public int LogId { get; set; }

        public string? UserAspId { get; set; }

        public string? Message { get; set; }

        public string? InnerException { get; set; }

        public string? StackTrace { get; set; }

        public string? DeveloperNote { get; set; }

        public DateTime DateEntered { get; set; }
    }
}
