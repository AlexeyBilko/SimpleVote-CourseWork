using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Answer : BaseEntity<int>
    {
        public string Value { get; set; }
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
