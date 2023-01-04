using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using ServiceLayer.DTO;

namespace ServiceLayer.Service.Abstraction
{
    public interface IFormService : IService<Form, FormDTO, int>
    {
        public Task<QuestionDTO> AddQuestion(QuestionDTO question);
    }
}
