using ServiceLayer.DTO;

namespace SimpleVote.UI.Models.ViewModels
{
    public class ReportViewModel
    {
        public List<QuestionDTO> Questions { get; set; }
        public string FormTitle { get; set; }
        public bool FormType { get; set; }
    }
}
