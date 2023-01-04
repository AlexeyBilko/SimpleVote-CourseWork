using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTO;
using SimpleVote.UI.Models.ViewModels;

namespace SimpleVote.UI.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(FormViewModel form)
        {
            if (ModelState.IsValid)
            {
                if (form != null)
                {
                    FormDTO _form = new FormDTO()
                    {
                        Name = form.Name,
                        Type = form.Type == "Анонімне" ? false : true,
                        TotalVoters = 0
                    };
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
