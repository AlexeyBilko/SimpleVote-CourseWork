using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DomainLayer.Models;
using Microsoft.Extensions.Options;
using RepositoryLayer.UnitOfWork;
using ServiceLayer.DTO;

namespace ServiceLayer.Service.Realization
{
    public class MyMapper
    {
        private readonly IUnitOfWork unitOfWork;
        IMapper mapper;

        public MyMapper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            MapperConfiguration configuration = new MapperConfiguration(opt =>
            {
                opt.CreateMap<User, UserDTO>();
                opt.CreateMap<UserDTO, User>();
            });
            mapper = new Mapper(configuration);
        }
        public async Task<AnswerDTO> AnswerToDTO(Answer toConvert)
        {
            return new AnswerDTO()
            {
                Id = toConvert.Id,
                Value = toConvert.Value,
                QuestionId = toConvert.QuestionId
            };
        }

        public Answer FromDTOtoAnswer(AnswerDTO toConvert)
        {
            return new Answer()
            {
                Id = toConvert.Id,
                Value = toConvert.Value,
                QuestionId = toConvert.QuestionId
            };
        }

        public async Task<VoteDTO> VoteToDTO(Vote toConvert)
        {
            Participant? participant = await unitOfWork.ParticipantRepository.Get((int)toConvert.ParticipantId);

            if (participant == null)
                throw new ArgumentException("participant not found for vote");

            return new VoteDTO()
            {
                Id = toConvert.Id,
                AnswerId = toConvert.AnswerId,
                Participant = await ParticipantToDTO(participant)
            };
        }

        public Vote FromDTOtoVote(VoteDTO toConvert)
        {
            return new Vote()
            {
                Id = toConvert.Id,
                AnswerId = toConvert.AnswerId,
                ParticipantId = toConvert.Participant.Id
            };
        }

        public async Task<ParticipantDTO> ParticipantToDTO(Participant toConvert)
        {
            return new ParticipantDTO()
            {
                Id = toConvert.Id,
                Name = toConvert.Name,
                Email = toConvert.Email,
                FormId = toConvert.FormId
            };
        }

        public Participant FromDTOtoParticipant(ParticipantDTO toConvert)
        {
            return new Participant()
            {
                Id = toConvert.Id,
                Name = toConvert.Name,
                Email = toConvert.Email,
                FormId = toConvert.FormId,
            };
        }

        public async Task<FormDTO> FormToDTO(Form toConvert)
        {
            var questions =  unitOfWork.QuestionRepository.GetAllQueryable().Where(form=>form.FormId == toConvert.Id);
            var participants = unitOfWork.ParticipantRepository.GetAllQueryable().Where(participant => participant.FormId == toConvert.Id);

            var questionsDTOs = new List<QuestionDTO>();
            foreach (var question in questions)
            {
                var converted = await QuestionToDTO(question);
                questionsDTOs.Add(converted);
            }
            var participantsDTOs = new List<ParticipantDTO>();
            foreach (var participant in participants)
            {
                var converted = await ParticipantToDTO(participant);
                participantsDTOs.Add(converted);
            }

            return new FormDTO()
            {
                Id = toConvert.Id,
                TotalVoters = toConvert.TotalVoters,
                Name = toConvert.Name,
                Type = toConvert.Type,
                Finished = toConvert.Finished,
                User = mapper.Map<User,UserDTO>(toConvert.User),
                Questions = questionsDTOs,
                Participants = participantsDTOs

            };
        }


        public Form FromDTOtoForm(FormDTO toConvert)
        {
            return new Form()
            {
                Id = toConvert.Id,
                Name = toConvert.Name,
                TotalVoters = toConvert.TotalVoters,
                Type = toConvert.Type,
                UserId = toConvert.User.Id,
                Finished = toConvert.Finished
            };
        }

        public async Task<QuestionDTO> QuestionToDTO(Question toConvert)
        {
            var answers = unitOfWork.AnswerRepository.GetAllQueryable()
                .Where(question => question.QuestionId == toConvert.Id);
            var votes = unitOfWork.VoteRepository.GetAllQueryable()
                .Where(vote => answers.Select(x=>x.Id).Contains(vote.AnswerId));

            var answerDTOs = new List<AnswerDTO>();
            foreach (var answer in answers)
            {
                var converted = await AnswerToDTO(answer);
                answerDTOs.Add(converted);
            }

            var voteDTOs = new List<VoteDTO>();
            foreach (var vote in votes)
            {
                var converted = await VoteToDTO(vote);
                voteDTOs.Add(converted);
            }

            return new QuestionDTO()
            {
                Id = toConvert.Id,
                Title = toConvert.Title,
                Type = toConvert.Type,
                FormId = toConvert.FormId,
                Answers = answerDTOs,
                Votes = voteDTOs
            };
        }

        public Question FromDTOtoQuestion(QuestionDTO toConvert)
        {
            return new Question()
            {
                Id = toConvert.Id,
                Title = toConvert.Title,
                Type = toConvert.Type,
                FormId = toConvert.FormId
            };
        }
    }

    public static class WhenAllHelper
    {
        public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return await Task.WhenAll(tasks);
        }
    }
}
