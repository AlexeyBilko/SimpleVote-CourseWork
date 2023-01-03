using DomainLayer.Models;
using RepositoryLayer.Repository;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        public IAnswerRepository AnswerRepository { get; set; }
        public IFormRepository FormRepository { get; set; }
        public IParticipantRepository ParticipantRepository { get; set; }
        public IQuestionRepository QuestionRepository { get; set; }
        public IVoteRepository VoteRepository { get; set; }

        public UnitOfWork(ApplicationDbContext context,
            IAnswerRepository _answerRepository,
            IFormRepository _formRepository,
            IParticipantRepository _participantRepository,
            IQuestionRepository _questionRepository,
            IVoteRepository _voteRepository)
        {
            this.context = context;
            AnswerRepository = _answerRepository;
            FormRepository = _formRepository;
            ParticipantRepository = _participantRepository;
            QuestionRepository = _questionRepository;
            VoteRepository = _voteRepository;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        ~UnitOfWork()
        {
            context.Dispose();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
