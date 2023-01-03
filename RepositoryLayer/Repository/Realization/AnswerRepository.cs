using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.Repository.Realization
{
    public class AnswerRepository : GenericRepository<Answer, int>, IAnswerRepository
    {
        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
