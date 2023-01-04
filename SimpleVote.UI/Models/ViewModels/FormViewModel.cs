using System.ComponentModel.DataAnnotations;
using ServiceLayer.DTO;

namespace SimpleVote.UI.Models.ViewModels
{
    public class FormViewModel
    {
        [Required]
        public string UserId { get; set; }
        public string Name { get; set; }
        public int FormId { get; set; } = 0;
        [Required(ErrorMessage = "TypeRequired")]
        public string Type { get; set; }
        public int TotalVoters { get; set; } = 0;
        public IEnumerable<QuestionDTO>? Questions { get; set; }
        public IEnumerable<ParticipantDTO>? Participants { get; set; }
        public IFormFile? participantsFile { get; set; }
    }
}
