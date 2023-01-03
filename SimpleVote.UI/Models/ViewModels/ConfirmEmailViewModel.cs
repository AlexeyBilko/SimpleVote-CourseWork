namespace SimpleVote.UI.Models.ViewModels
{
    public class ConfirmEmailViewModel
    {
        public string? Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Code { get; set; }
        public string? ConfirmCode { get; set; }
        public bool IsRestorePassword { get; set; } = false;
    }
}
