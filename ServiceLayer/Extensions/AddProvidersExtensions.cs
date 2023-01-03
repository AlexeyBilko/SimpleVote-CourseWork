using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer;
using RepositoryLayer.Repository;
using ServiceLayer.DTO;
using ServiceLayer.Service.Abstraction;
using ServiceLayer.Service.Realization;
using ServiceLayer.Service.Realization.IdentityServices;
using ServiceLayer.Services.IdentityServices;

namespace ServiceLayer.Extensions
{
    public static class AddProvidersExtensions
    {
        public static void ConfigureIdentityOptions(this IServiceCollection services, string connectionStr)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                //string eng = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //string ukr = "абвгґдеєжзиіїйклмнопрстуфхцчшщАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩ";
                options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail = true;
            });
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }
        public static void AddServicesDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<AnswerService, AnswerService>();

            services.AddScoped<IFormService, FormService>();
            services.AddScoped<FormService, FormService>();

            services.AddScoped<IParticipantService, ParticipantService>();
            services.AddScoped<ParticipantService, ParticipantService>();

            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<QuestionService, QuestionService>();

            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<VoteService, VoteService>();

            services.AddScoped<UserService, UserService>();
            services.AddScoped<SignInService, SignInService>();
            services.AddScoped<RoleService, RoleService>();

            services.AddTransient<DbContext, ApplicationDbContext>();
        }
    }
}
