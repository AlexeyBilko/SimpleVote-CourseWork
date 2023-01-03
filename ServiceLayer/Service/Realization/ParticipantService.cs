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
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyMapper mapper;

        public ParticipantService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MyMapper(this.unitOfWork);
        }

        public async Task<ParticipantDTO> AddAsync(ParticipantDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoParticipant(entity);
            var result = await unitOfWork.ParticipantRepository.CreateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.ParticipantToDTO(result);
        }

        public async Task<ParticipantDTO> DeleteAsync(ParticipantDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoParticipant(entity);

            var result = await unitOfWork.ParticipantRepository.DeleteAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.ParticipantToDTO(result);
        }

        public async Task<ParticipantDTO> DeleteById(int id)
        {
            var result = await unitOfWork.ParticipantRepository.DeleteById(id);
            unitOfWork.SaveChanges();

            return await mapper.ParticipantToDTO(result);
        }

        public async Task<ParticipantDTO> UpdateAsync(ParticipantDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoParticipant(entity);
            var result = await unitOfWork.ParticipantRepository.UpdateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.ParticipantToDTO(result);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetAllAsync()
        {
            return await (await unitOfWork.ParticipantRepository.GetAllAsync())
                .Select(async x=> await mapper.ParticipantToDTO(x)).WhenAll();
        }

        public async Task<ParticipantDTO> GetAsync(int id)
        {
            var answer = await unitOfWork.ParticipantRepository.Get(id);
            if (answer != null)
            {
                return await mapper.ParticipantToDTO(answer);
            }
            throw new ArgumentException("answer not found");
        }
    }
}
