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

        public async Task<IActionResult> MyForms()
        {
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
            return View(model);

        }

        public async Task<IActionResult> MyFormsList(string userId, int pageNumber = 1)
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