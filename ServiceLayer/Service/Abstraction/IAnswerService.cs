using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using ServiceLayer.DTO;

namespace ServiceLayer.Service.Abstraction
{
    public interface IAnswerService : IService<Answer, AnswerDTO, int>
    {
    }
}
