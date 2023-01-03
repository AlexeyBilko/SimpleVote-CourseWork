using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer;
using RepositoryLayer.Repository;
using RepositoryLayer.Repository.Abstraction;
using RepositoryLayer.Repository.Realization;
using RepositoryLayer.UnitOfWork;

namespace ServiceLayer.Extensions
{
    public static class DependenciesInjection
    {
        public static void AddAppDbContext(this IServiceCollection services, string connectionStr)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionStr);
            });
        }
        public static void AddRepositoryDependencies(this IServiceCollection services)
        {
            services.AddScoped<DbContext, ApplicationDbContext>();

            services.AddScoped<IAnswerRepository, AnswerRepository>();
            
            services.AddScoped<IFormRepository, FormRepository>();
            
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            
            services.AddScoped<IVoteRepository, VoteRepository>();

            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
