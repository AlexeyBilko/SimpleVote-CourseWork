using System.ComponentModel.DataAnnotations;
using ServiceLayer.DTO;

namespace SimpleVote.UI.Models.ViewModels
{
    public class QuestionViewModel
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int FormId { get; set; }
        public List<string>? Answers { get; set; }
    }
}
