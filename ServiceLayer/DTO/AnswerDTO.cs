using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class AnswerDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int QuestionId { get; set; }
        public IEnumerable<VoteDTO> Votes { get; set; }
    }
}
