using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.Repository.Realization
{
    public class VoteRepository : GenericRepository<Vote, int>, IVoteRepository
    {
        public VoteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
