using ServiceLayer.DTO;

namespace SimpleVote.UI.Models.ViewModels
{
    public class ShowFormViewModel
    {
        public FormDTO toShow { get; set; }
        public List<string>? votes { get; set; }
        public ParticipantDTO? Participant { get; set; }
    }
}
