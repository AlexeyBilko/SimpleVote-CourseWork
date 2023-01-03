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
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyMapper mapper;

        public AnswerService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MyMapper(this.unitOfWork);
        }

        public async Task<AnswerDTO> AddAsync(AnswerDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoAnswer(entity);
            var result = await unitOfWork.AnswerRepository.CreateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.AnswerToDTO(result);
        }

        public async Task<AnswerDTO> DeleteAsync(AnswerDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoAnswer(entity);

            var result = await unitOfWork.AnswerRepository.DeleteAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.AnswerToDTO(result);
        }

        public async Task<AnswerDTO> DeleteById(int id)
        {
            var result = await unitOfWork.AnswerRepository.DeleteById(id);
            unitOfWork.SaveChanges();

            return await mapper.AnswerToDTO(result);
        }

        public async Task<AnswerDTO> UpdateAsync(AnswerDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoAnswer(entity);
            var result = await unitOfWork.AnswerRepository.UpdateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.AnswerToDTO(result);
        }

        public async Task<IEnumerable<AnswerDTO>> GetAllAsync()
        {
            return await (await unitOfWork.AnswerRepository.GetAllAsync())
                .Select(async x=> await mapper.AnswerToDTO(x)).WhenAll();
        }

        public async Task<AnswerDTO> GetAsync(int id)
        {
            var answer = await unitOfWork.AnswerRepository.Get(id);
            if (answer != null)
            {
                return await mapper.AnswerToDTO(answer);
            }
            throw new ArgumentException("answer not found");
        }
    }
}
