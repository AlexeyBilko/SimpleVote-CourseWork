using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace RepositoryLayer.Repository.Abstraction
{
    public interface IVoteRepository : IRepository<Vote, int>
    {
    }
}
