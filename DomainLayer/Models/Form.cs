using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Form : BaseEntity<int>
    {
        public string Name { get; set; }
        public int TotalVoters { get; set; }
        public bool Type { get; set; } //0 - anonymous, 1 - group
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
