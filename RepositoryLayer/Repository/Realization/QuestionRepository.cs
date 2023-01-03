using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.Repository.Realization
{
    public class QuestionRepository : GenericRepository<Question, int>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
