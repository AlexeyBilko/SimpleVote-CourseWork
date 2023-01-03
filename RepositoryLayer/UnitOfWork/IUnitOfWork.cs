using RepositoryLayer.Repository;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IAnswerRepository AnswerRepository { get; set; }
        public IFormRepository FormRepository { get; set; }
        public IParticipantRepository ParticipantRepository { get; set; }
        public IQuestionRepository QuestionRepository { get; set; }
        public IVoteRepository VoteRepository { get; set; }
        public void SaveChanges();
    }
}
