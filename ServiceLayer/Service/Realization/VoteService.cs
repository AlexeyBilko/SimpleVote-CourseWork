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
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyMapper mapper;

        public VoteService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MyMapper(this.unitOfWork);
        }

        public async Task<VoteDTO> AddAsync(VoteDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoVote(entity);
            var result = await unitOfWork.VoteRepository.CreateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.VoteToDTO(result);
        }

        public async Task<VoteDTO> DeleteAsync(VoteDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoVote(entity);

            var result = await unitOfWork.VoteRepository.DeleteAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.VoteToDTO(result);
        }

        public async Task<VoteDTO> DeleteById(int id)
        {
            var result = await unitOfWork.VoteRepository.DeleteById(id);
            unitOfWork.SaveChanges();

            return await mapper.VoteToDTO(result);
        }

        public async Task<VoteDTO> UpdateAsync(VoteDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoVote(entity);
            var result = await unitOfWork.VoteRepository.UpdateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.VoteToDTO(result);
        }

        public async Task<IEnumerable<VoteDTO>> GetAllAsync()
        {
            return await (await unitOfWork.VoteRepository.GetAllAsync())
                .Select(async x=> await mapper.VoteToDTO(x)).WhenAll();
        }

        public async Task<VoteDTO> GetAsync(int id)
        {
            var answer = await unitOfWork.VoteRepository.Get(id);
            if (answer != null)
            {
                return await mapper.VoteToDTO(answer);
            }
            throw new ArgumentException("answer not found");
        }
    }
}
