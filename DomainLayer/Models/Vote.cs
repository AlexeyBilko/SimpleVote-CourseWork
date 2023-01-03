using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Vote : BaseEntity<int>
    {
        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public int? ParticipantId { get; set; }
        public virtual Participant? Voter { get; set; }

    }
}
