using LeadSub.Models;
using LeadSub.Models.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTO;
using ServiceLayer.Service.Realization.IdentityServices;
using SimpleVote.UI.Models.ViewModels;

namespace SimpleVote.UI.Controllers
{
    public class AccountController : Controller
    {
        UserService userService;
        SignInService signInService;
        public IConfiguration Configuration { get; }
        EmailConfiguration emailConfiguration;

        public AccountController(UserService userService,
            SignInService signInServcie,
            EmailConfiguration emailConfiguration,
            IConfiguration configuration)
        {
            this.userService = userService;
            this.signInService = signInServcie;
            this.emailConfiguration = emailConfiguration;
            this.Configuration = configuration;
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return RedirectToAction("Login", new { returnUrl = ReturnUrl });
        }

        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var email = (await userService.GetUser(User)).Email;
                    var res = await userService.ChangePasswordAsync(email, model.Password, model.ConfirmNewPassword);
                    if (res.Succeeded)
                    {
                        TempData["Message"] = "Пароль успішно змінено";
                    }
                    else
                    {
                        TempData["Message"] = "";
                        int i = 1;
                        foreach (var item in res.Errors)
                        {
                            TempData["Message"] += i.ToString() + ". " + item.Code + "\n";
                            i++;
                        }
                    }
                }
                else
                {
                    TempData["Message"] = "Помилка зміни паролю";
                }

                return RedirectToAction("ForgotPassword", "Account");
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }
        //public IActionResult EnterEmail()
        //{
        //    return View();
        //}

        //[Authorize]
        //public async Task<IActionResult> ForgotPassword(string?Email)
        //{
        //    UserDTO user=new UserDTO();
        //    if (Email != null)
        //    {
        //        user = await userService.FindByEmailAsync(Email);
        //        if (user == null)
        //        {
        //            TempData["Message"] = "Account Not Found"];     
        //            return RedirectToAction("EnterEmail");
        //        }
        //    }
        //    else
        //    {
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            user= await userService.GetUser(User);
        //        }
        //        else
        //        {
        //            return RedirectToAction("EnterEmail");
        //        }
        //    }
        //    Random rand = new Random();
        //    int code = rand.Next(100000, 999999);

        //    EmailMessage message = new EmailMessage
        //    {
        //        To = user.Email,
        //        Subject = "Verification Code",
        //        Content = code.ToString()
        //    };
        //    await EmailManager.SendText(emailConfiguration,message);

        //    return View("ConfirmEmail", new ConfirmEmailViewModel
        //    {
        //        Code = code.ToString(),
        //        Email=user.Email,
        //        IsRestorePassword=true
        //    });

        //}
        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(ConfirmEmailViewModel model)
        //{
        //    if (model.ConfirmCode == model.Code)
        //    {
        //        return View("RestorePassword", new RestorePasswordViewModel()
        //        {
        //            Email = model.Email
        //        });
        //    }
        //    else
        //    {
        //        TempData["Message"] = localizer["VerificationError"].ToString();
        //        return View("ConfirmEmail", model);
        //    }
        //}




        //[Authorize]
        //public async Task<IActionResult> AccountInfo()
        //{
        //    UserDTO user = await userService.GetUser(User);
        //    UserPlanDTO userPlan = await UserPlanService.GetAsync(user.UserPlanId);
        //    AccountSettingsViewModel model = new AccountSettingsViewModel
        //    {
        //        Name = user.DisplayName,
        //        Email = user.Email,
        //        TotalFollowers = 0,
        //        UserPlanTitle=userPlan.Title
        //    };
        //    return View(model);
        //}
        //public IActionResult Register()
        //{
        //    return View();
        //}

        public IActionResult ConfirmEmail()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserDTO user = await userService.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        string role = await userService.GetUserRoleAsync(user.Id);
                        var result = await signInService.SignInWithEmailAsync(model.Email, model.Password);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Message"] = "Incorrect Login";
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Incorrect Login";
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityResult[] passwordValidation = await userService.ValidatePassword(model.Password);
                    if (passwordValidation[0].Succeeded)
                    {
                        if (await userService.FindByEmailAsync(model.Email) == null)
                        {
                            Random rand = new Random();
                            int code = rand.Next(100000, 999999);

                            EmailMessage message = new EmailMessage
                            {
                                To = model.Email,
                                Subject = "Veryfication Code",
                                Content = code.ToString()
                            };
                            try
                            {
                                await EmailManager.SendText(emailConfiguration, message);
                            }
                            catch
                            {
                                return View("ConfirmEmail", new ConfirmEmailViewModel
                                {
                                    UserName = model.UserName,
                                    Email = model.Email,
                                    Password = model.Password,
                                    Code = code.ToString()
                                });
                            }

                            return View("ConfirmEmail", new ConfirmEmailViewModel
                            {
                                UserName = model.UserName,
                                Email = model.Email,
                                Password = model.Password,
                                Code = code.ToString()
                            });
                        }
                        else
                        {
                            TempData["Message"] = "This email has already been used";
                        }
                    }
                    else
                    {
                        TempData["Message"] = "";
                        int i = 1;
                        foreach (var item in passwordValidation[0].Errors)
                        {
                            TempData["Message"] += i.ToString() + ". " + item.Code + "\n";
                            i++;
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model,
            [FromServices] IServiceProvider serviceProvider)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ConfirmCode == model.Code)
                    {
                        UserDTO user = new UserDTO
                        {
                            Email = model.Email,
                            DisplayName = model.UserName
                        };
                        var result = await userService.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                            var roleExist = await RoleManager.RoleExistsAsync("User");
                            if (!roleExist)
                            {
                                var roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
                            }

                            var roleRes = await userService.AddToRoleAsync(user.Id, "User");
                            if (roleRes.Succeeded)
                            {
                                await signInService.SignInWithEmailAsync(model.Email, model.Password);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("", item.Code);
                            }
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Verification Error";
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> AppLogout()
        {
            await signInService.SignOut();
            return RedirectToAction("Index", "Home");
        }
        
    }
}
