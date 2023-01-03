using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Participant : BaseEntity<int>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int FormId { get; set; }
        public virtual Form Form { get; set; }
    }
}
