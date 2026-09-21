namespace DraughtSurveyWebApp.ViewModels
{
    public sealed class FeedbackRequestDto
    {
        public string AppKey { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? SenderEmail { get; set; }
        public int Type { get; set; } = 1; // ConTactFormApi enum, type of feedback, General = 1
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
