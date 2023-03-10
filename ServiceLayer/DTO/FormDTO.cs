using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class FormDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalVoters { get; set; }
        public bool Type { get; set; }
        public bool Finished { get; set; }
        public UserDTO User { get; set; }
        public IEnumerable<QuestionDTO>? Questions { get; set; }
        public IEnumerable<ParticipantDTO>? Participants { get; set; }
    }
}
