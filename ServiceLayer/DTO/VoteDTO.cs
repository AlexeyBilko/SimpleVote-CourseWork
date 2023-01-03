using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class VoteDTO
    {
        public int Id { get; set; }
        public AnswerDTO Answer { get; set; }
        public ParticipantDTO Participants { get; set; }
    }
}
