using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RepositoryLayer.UnitOfWork;
using ServiceLayer.DTO;
using ServiceLayer.Service.Abstraction;

namespace ServiceLayer.Service.Realization
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyMapper mapper;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MyMapper(this.unitOfWork);
        }

        public async Task<QuestionDTO> AddAsync(QuestionDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoQuestion(entity);
            var result = await unitOfWork.QuestionRepository.CreateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.QuestionToDTO(result);
        }

        public async Task<QuestionDTO> DeleteAsync(QuestionDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoQuestion(entity);

            var result = await unitOfWork.QuestionRepository.DeleteAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.QuestionToDTO(result);
        }

        public async Task<QuestionDTO> DeleteById(int id)
        {
            var result = await unitOfWork.QuestionRepository.DeleteById(id);
            unitOfWork.SaveChanges();

            return await mapper.QuestionToDTO(result);
        }

        public async Task<QuestionDTO> UpdateAsync(QuestionDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoQuestion(entity);
            var result = await unitOfWork.QuestionRepository.UpdateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.QuestionToDTO(result);
        }

        public async Task<IEnumerable<QuestionDTO>> GetAllAsync()
        {
            return await (await unitOfWork.QuestionRepository.GetAllAsync())
                .Select(async x=> await mapper.QuestionToDTO(x)).WhenAll();
        }

        public async Task<QuestionDTO> GetAsync(int id)
        {
            var answer = await unitOfWork.QuestionRepository.Get(id);
            if (answer != null)
            {
                return await mapper.QuestionToDTO(answer);
            }
            throw new ArgumentException("answer not found");
        }
    }
}
