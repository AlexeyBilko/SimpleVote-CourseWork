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
        public List<string> SubmitedAnswer { get; set; }
        public int QuestionId { get; set; }
        public ParticipantDTO? Participant { get; set; }
    }
}
