using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Question : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int FormId { get; set; }
        public virtual Form Form { get; set; }
    }
}
