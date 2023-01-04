using ServiceLayer.DTO;

namespace SimpleVote.UI.Models.ViewModels
{
    public class MySubPagesListViewModel
    {
        public IEnumerable<FormDTO> forms { get; set; }
        public string UserId { get; set; }
        public List<int> FinishedFormsIDs { get; set; }
    }
}
