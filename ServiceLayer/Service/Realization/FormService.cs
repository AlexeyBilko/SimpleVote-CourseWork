﻿using System;
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
    public class FormService : IFormService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MyMapper mapper;

        public FormService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MyMapper(this.unitOfWork);
        }

        public async Task<FormDTO> AddAsync(FormDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoForm(entity);
            var result = await unitOfWork.FormRepository.CreateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.FormToDTO(result);
        }

        public async Task<FormDTO> DeleteAsync(FormDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoForm(entity);

            var result = await unitOfWork.FormRepository.DeleteAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.FormToDTO(result);
        }

        public async Task<FormDTO> DeleteById(int id)
        {
            var result = await unitOfWork.FormRepository.DeleteById(id);
            unitOfWork.SaveChanges();

            return await mapper.FormToDTO(result);
        }

        public async Task<FormDTO> UpdateAsync(FormDTO entity)
        {
            var mappedEntity = mapper.FromDTOtoForm(entity);
            var result = await unitOfWork.FormRepository.UpdateAsync(mappedEntity);
            unitOfWork.SaveChanges();

            return await mapper.FormToDTO(result);
        }

        public async Task<IEnumerable<FormDTO>> GetAllAsync()
        {
            return await (await unitOfWork.FormRepository.GetAllAsync())
                .Select(async x=> await mapper.FormToDTO(x)).WhenAll();
        }

        public async Task<FormDTO> GetAsync(int id)
        {
            var answer = await unitOfWork.FormRepository.Get(id);
            if (answer != null)
            {
                return await mapper.FormToDTO(answer);
            }
            throw new ArgumentException("answer not found");
        }
    }
}