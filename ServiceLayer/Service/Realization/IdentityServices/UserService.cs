using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using ServiceLayer.DTO;

namespace ServiceLayer.Service.Realization.IdentityServices
{
    public class UserService
    {
        UserManager<User> userManager;
        IMapper mapper;

        public UserService(UserManager<User> userManager)
        {
            this.userManager = userManager;
            MapperConfiguration configuration = new MapperConfiguration(opt =>
            {
                opt.CreateMap<User, UserDTO>();
                opt.CreateMap<UserDTO, User>();
            });
            mapper = new Mapper(configuration);
        }
        
        public async Task<IdentityResult> CreateAsync(UserDTO user, string password)
        {
            user.Id = Guid.NewGuid().ToString();
            User newUser = mapper.Map<UserDTO, User>(user);
            newUser.UserName = Guid.NewGuid().ToString();
            var res = await userManager.CreateAsync(newUser, password);
            return res;
        }

        public async Task<UserDTO> GetUser(ClaimsPrincipal claims)
        {
            UserDTO user = mapper.Map<User, UserDTO>(await userManager.GetUserAsync(claims));
            return user;
        }

        public async Task<UserDTO> FindByEmailAsync(string email)
        {
            UserDTO user = mapper.Map<User, UserDTO>(await userManager.FindByEmailAsync(email));
            return user;
        }

        public string GetUserId(ClaimsPrincipal claims)
        {
            return userManager.GetUserId(claims);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string Email, string newPassword, string oldPassword)
        {
            User user = await userManager.FindByEmailAsync(Email);
            var res = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return res;
        }

        public async Task<IdentityResult> RestorePassword(string email, string newPassword)
        {
            User user = await userManager.FindByEmailAsync(email);
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            PasswordVerificationResult res = hasher.VerifyHashedPassword(user, user.PasswordHash, newPassword);
            if (res == PasswordVerificationResult.Failed)
            {
                string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var identityRes = await userManager.ResetPasswordAsync(user, resetToken, newPassword);
                return identityRes;
            }
            IdentityError error = new IdentityError();
            error.Code = "OldPasswordMustNotMatch";
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            User user = await userManager.FindByIdAsync(userId);
            var res = await userManager.AddToRoleAsync(user, role);
            return res;
        }

        public Task<UserDTO[]> GetAllUsersAsync()
        {
            return Task.Run(() =>
            {
                return userManager
                    .Users
                    .Select(x => mapper.Map<User, UserDTO>(x))
                    .ToArray();
            });
        }

        public Task<string> GetUserRoleAsync(string userId)
        {
            return Task.Run(async () =>
            {
                User user = await userManager.FindByIdAsync(userId);
                IList<string> roles = await userManager.GetRolesAsync(user);
                if (roles.Count != 0)
                {
                    return roles[0];
                }
                return "";
            });
        }

        public Task<UserDTO> FindUserById(string userId)
        {
            return Task.Run(async () =>
            {
                UserDTO user = mapper.Map<User, UserDTO>(await userManager.FindByIdAsync(userId));
                return user;
            });
        }

        public Task<IdentityResult> RemoveFromRoleAsync(string userId, string currentRole)
        {
            return Task.Run(async () =>
            {
                User user = await userManager.FindByIdAsync(userId);
                var res= await userManager.RemoveFromRoleAsync(user, currentRole);
                return res;
            });
        }

        public async Task<IdentityResult[]>ValidatePassword(string password)
        {
            List<IdentityResult> resList = new List<IdentityResult>();
            foreach (var validator in userManager.PasswordValidators)
            {
                IdentityResult res= await validator.ValidateAsync(userManager,null,password);
                resList.Add(res);
            }
            return resList.ToArray();
        }
    }
}
