using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.Repository.Realization
{
    public class FormRepository : GenericRepository<Form, int>, IFormRepository
    {
        public FormRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
