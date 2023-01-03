using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int FormId { get; set; }
        public IEnumerable<AnswerDTO> Answers { get; set; }
        public IEnumerable<VoteDTO> Votes { get; set; }
    }
}
