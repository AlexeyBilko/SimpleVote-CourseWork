using Microsoft.AspNetCore.Mvc;
using SimpleVote.UI.Models;
using System.Diagnostics;
using ServiceLayer.DTO;
using ServiceLayer.Service.Realization;
using ServiceLayer.Service.Realization.IdentityServices;
using SimpleVote.UI.Models.ViewModels;

namespace SimpleVote.UI.Controllers
{
    public class HomeController : Controller
    {

        FormService formService;
        UserService userManager;

        public HomeController(FormService _formService, UserService user)
        {
            formService = _formService;
            userManager = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("error")]
        public IActionResult NotFoundPage()
        {
            return View();
        }


        [Route("CopyLink")]
        public IActionResult CopyLink(int? id)
        {
            try
            {
                TextCopy.ClipboardService.SetText($"https://localhost:7219/form?id={id}");
                return RedirectToAction("MyForms", "Home", new { copy = "true" });
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> MyForms(string? copy = "")
        {
            try
            {
                if (User.IsInRole("User") == false)
                {
                    TempData["Message"] =
                        "Нажаль у вас немає доступу до цієї сторінки, будь-ласка зареєструйтеся або увійдіть в свій облікови запис";
                    return RedirectToAction("Index", "Home");
                }
                MyFormsViewModel model = new MyFormsViewModel();
                UserDTO user = await userManager.GetUser(User);
                int totalCount = (await formService.GetFormsbyUserId(user.Id)).Count;
                model.UserId = user.Id;
                model.Pagination = new PaginationModel
                {
                    TotalCount = totalCount,
                    PageSize = 3
                };
                model.Pagination.CalculateQuantityPages();
                model.Pagination.CalculateQuantityPaginationContainers();
                if (copy == "true")
                {
                    TempData["Message"] = "Посилання на опитування успішно скопійовано";
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

        public async Task<IActionResult> MyFormsList(string userId, int pageNumber = 1)
        {
            try
            {
                MyFormsListViewModel vm = new MyFormsListViewModel();
                vm.forms = (await formService.GetFormsbyUserId(userId))
                    .Skip((pageNumber - 1) * 3)
                    .Take(3);
                vm.UserId = userId;

                List<int> FinishedFormsIDs = new List<int>();
                foreach (var item in vm.forms)
                {
                    if (item.Finished)
                    {
                        FinishedFormsIDs.Add(item.Id);
                    }
                }

                vm.FinishedFormsIDs = FinishedFormsIDs;

                return PartialView("MyFormsList", vm);
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}